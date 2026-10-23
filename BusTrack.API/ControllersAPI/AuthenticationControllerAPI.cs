
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.ClassesDB;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using BCryptNet = BCrypt.Net.BCrypt;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationControllerAPI : ControllerBase
    {
        private const string ProtectedAdminEmail = "admin@admin.com";
        private const int PasswordHistoryLimit = 5;

        private const string GenericRecoveryError =
            "Não foi possível concluir a recuperação. " +
            "Verifique os dados informados.";

        private readonly IUserAuthenticationServiceAPI _userService;
        private readonly IJwtTokenServiceAPI _jwtTokenService;
        private readonly IUserRepositoryDB _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<BsonDocument> _bootstrapLocks;
        private readonly IMongoCollection<BsonDocument> _sessions;
        private readonly IMongoCollection<BsonDocument> _passwordRecoveries;
        private readonly IMongoCollection<BsonDocument> _passwordAdministrationAudit;

        public AuthenticationControllerAPI(
            IUserAuthenticationServiceAPI userService,
            IJwtTokenServiceAPI jwtTokenService,
            IUserRepositoryDB userRepository,
            IConfiguration configuration,
            IMongoDatabase database)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
            _userRepository = userRepository;
            _configuration = configuration;

            _bootstrapLocks = database.GetCollection<BsonDocument>(
                "BusTrackSecurityLocks");

            _sessions = database.GetCollection<BsonDocument>(
                "BusTrackActiveSessions");

            _passwordRecoveries = database.GetCollection<BsonDocument>(
                "BusTrackPasswordRecovery");

            _passwordAdministrationAudit = database.GetCollection<BsonDocument>(
                "BusTrackPasswordAdministrationAudit");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequestDTOAPI request,
            CancellationToken cancellationToken)
        {
            var email = request.Email?.Trim() ?? string.Empty;
            var password = request.Password ?? string.Empty;

            var user = _userService.Authenticate(email, password);

            if (user is null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "E-mail ou senha incorretos."
                });
            }

            var now = DateTime.UtcNow;

            if (user.PasswordChangedAtUtc is null)
            {
                user.PasswordChangedAtUtc = now;

                if (string.IsNullOrWhiteSpace(user.Id) ||
                    !_userRepository.Update(user.Id, user))
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        message =
                            "Não foi possível validar a política de senha."
                    });
                }
            }

            var expirationDate =
                PasswordPolicyServiceAPI.GetExpirationDate(user);

            if (PasswordPolicyServiceAPI.IsPastGracePeriod(user, now))
            {
                return StatusCode(403, new
                {
                    success = false,
                    code = "PASSWORD_EXPIRED",
                    message =
                        "Sua senha expirou. Altere sua senha para continuar."
                });
            }

            var browserId = Request.Headers["X-Browser-Id"].ToString();

            if (!Guid.TryParse(browserId, out var parsedBrowserId))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Identificador do navegador inválido."
                });
            }

            browserId = parsedBrowserId.ToString("N");
            var sessionId = Guid.NewGuid().ToString("N");

            var (accessToken, expiresAtUtc) =
                _jwtTokenService.CreateToken(user, browserId, sessionId);

            var session = new BsonDocument
            {
                ["_id"] = browserId,
                ["sessionId"] = sessionId,
                ["userId"] = user.Id,
                ["email"] = user.Email ?? string.Empty,
                ["fullName"] = user.FullName ?? string.Empty,
                ["role"] = string.Equals(
                    user.Role,
                    "ADMIN",
                    StringComparison.OrdinalIgnoreCase)
                    ? "ADMIN"
                    : "USER",
                ["lastSeenUtc"] = new BsonDateTime(now),
                ["expiresAtUtc"] = new BsonDateTime(expiresAtUtc)
            };

            await _sessions.ReplaceOneAsync(
                Builders<BsonDocument>.Filter.Eq("_id", browserId),
                session,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken);

            return Ok(new AuthenticationResponseDTOAPI
            {
                Success = true,
                AccessToken = accessToken,
                ExpiresAtUtc = expiresAtUtc,
                WelcomeMessage =
                    "Sucesso. Seja bem-vindo ao painel principal do Bus Track.",
                PasswordExpiresAtUtc = expirationDate,
                PasswordDaysUntilExpiration =
                    PasswordPolicyServiceAPI.GetDaysUntilExpiration(user, now),
                PasswordExpirationWarning =
                    PasswordPolicyServiceAPI.IsExpirationWarningDue(user, now)
            });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword(
            [FromBody] ForgotPasswordRequestDTOAPI request)
        {
            var email = request.Email?.Trim() ?? string.Empty;
            var newPassword = request.NewPassword ?? string.Empty;
            var confirmPassword = request.ConfirmPassword ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Preencha os três campos corretamente."
                });
            }

            var user = _userRepository.GetByEmail(email);

            // Bloqueia admin@admin.com e qualquer conta com perfil ADMIN.
            // Conta administrativa e conta inexistente recebem a mesma
            // resposta, sem alterar senha, histórico ou sessões.
            if (user is null ||
                IsProtectedAdminEmail(email) ||
                PasswordPolicyServiceAPI.IsProtectedAdministrator(user) ||
                string.Equals(
                    user.Role,
                    "ADMIN",
                    StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(user.Id))
            {
                return GenericRecoveryFailure();
            }

            if (!string.Equals(
                newPassword,
                confirmPassword,
                StringComparison.Ordinal))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "As senhas não coincidem."
                });
            }

            if (!PasswordPolicyServiceAPI.IsValid(newPassword))
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        PasswordPolicyServiceAPI.GetValidationMessage()
                });
            }

            if (PasswordPolicyServiceAPI.MatchesCurrentOrHistory(
                newPassword,
                user))
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "A nova senha não pode ser igual à senha atual " +
                        "nem a uma das cinco senhas anteriores."
                });
            }

            // Atualiza a senha apenas depois das validações.
            PasswordPolicyServiceAPI.ApplyNewPassword(user, newPassword);

            if (!_userRepository.Update(user.Id, user))
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = GenericRecoveryError
                });
            }

            // Invalida todas as sessões ativas após a recuperação.
            _ = _sessions.DeleteManyAsync(
                Builders<BsonDocument>.Filter.Eq("userId", user.Id));

            return Ok(new
            {
                success = true,
                message =
                    "Senha alterada com sucesso. Entre novamente."
            });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordRequestDTOAPI request,
            CancellationToken cancellationToken)
        {
            var email = request.Email?.Trim() ?? string.Empty;
            var recoveryCode = request.RecoveryCode?.Trim() ?? string.Empty;
            var newPassword = request.NewPassword ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(recoveryCode) ||
                string.IsNullOrWhiteSpace(newPassword))
            {
                return GenericRecoveryFailure();
            }

            var user = _userRepository.GetByEmail(email);

            if (user is null ||
                IsProtectedAdminEmail(email) ||
                PasswordPolicyServiceAPI.IsProtectedAdministrator(user) ||
                string.Equals(
                    user.Role,
                    "ADMIN",
                    StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(user.Id))
            {
                return GenericRecoveryFailure();
            }

            if (!PasswordPolicyServiceAPI.IsValid(newPassword))
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        PasswordPolicyServiceAPI.GetValidationMessage()
                });
            }

            if (PasswordPolicyServiceAPI.MatchesCurrentOrHistory(
                newPassword,
                user))
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "A nova senha não pode ser igual à senha atual " +
                        "nem a uma das cinco senhas anteriores."
                });
            }

            var normalizedEmail = email.ToLowerInvariant();
            var codeHash = HashRecoveryCode(recoveryCode);
            var now = DateTime.UtcNow;

            var recoveryFilter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq(
                    "emailNormalized",
                    normalizedEmail),
                Builders<BsonDocument>.Filter.Eq(
                    "codeHash",
                    codeHash),
                Builders<BsonDocument>.Filter.Eq("used", false),
                Builders<BsonDocument>.Filter.Gt("expiresAtUtc", now));

            var recovery = await _passwordRecoveries
                .Find(recoveryFilter)
                .FirstOrDefaultAsync(cancellationToken);

            if (recovery is null)
            {
                return GenericRecoveryFailure();
            }

            // Consumo atômico para impedir reutilização do código.
            var consumeFilter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq(
                    "_id",
                    recovery["_id"]),
                Builders<BsonDocument>.Filter.Eq("used", false),
                Builders<BsonDocument>.Filter.Gt("expiresAtUtc", now));

            var consumeResult = await _passwordRecoveries.UpdateOneAsync(
                consumeFilter,
                Builders<BsonDocument>.Update
                    .Set("used", true)
                    .Set("usedAtUtc", now),
                cancellationToken: cancellationToken);

            if (consumeResult.ModifiedCount != 1)
            {
                return GenericRecoveryFailure();
            }

            PasswordPolicyServiceAPI.ApplyNewPassword(user, newPassword);

            if (!_userRepository.Update(user.Id, user))
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = GenericRecoveryError
                });
            }

            await _sessions.DeleteManyAsync(
                Builders<BsonDocument>.Filter.Eq("userId", user.Id),
                cancellationToken);

            return Ok(new
            {
                success = true,
                message =
                    "Senha redefinida com sucesso. Entre novamente."
            });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("change-password")]
        public IActionResult ChangePassword(
            [FromBody] ChangePasswordRequestDTOAPI request)
        {
            var email = request.Email?.Trim() ?? string.Empty;
            var currentPassword = request.CurrentPassword ?? string.Empty;
            var newPassword = request.NewPassword ?? string.Empty;
            var confirmPassword = request.ConfirmPassword ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(currentPassword) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Preencha todos os campos."
                });
            }

            var user = _userRepository.GetByEmail(email);

            if (user is null ||
                IsProtectedAdminEmail(email) ||
                PasswordPolicyServiceAPI.IsProtectedAdministrator(user) ||
                string.Equals(
                    user.Role,
                    "ADMIN",
                    StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Não foi possível alterar a senha com os dados informados."
                });
            }

            if (string.IsNullOrWhiteSpace(user.Password) ||
                !BCryptNet.Verify(currentPassword, user.Password))
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "Não foi possível alterar a senha com os dados informados."
                });
            }

            if (!string.Equals(
                newPassword,
                confirmPassword,
                StringComparison.Ordinal))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "A nova senha e a confirmação são diferentes."
                });
            }

            if (!PasswordPolicyServiceAPI.IsValid(newPassword))
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        PasswordPolicyServiceAPI.GetValidationMessage()
                });
            }

            if (PasswordPolicyServiceAPI.MatchesCurrentOrHistory(
                newPassword,
                user))
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        "A nova senha não pode ser igual à senha atual " +
                        "nem a uma das cinco senhas anteriores."
                });
            }

            PasswordPolicyServiceAPI.ApplyNewPassword(user, newPassword);

            if (string.IsNullOrWhiteSpace(user.Id) ||
                !_userRepository.Update(user.Id, user))
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Não foi possível salvar a nova senha."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Senha alterada com sucesso."
            });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("admin-reset-password")]
        public async Task<IActionResult> AdminResetPassword(
            [FromBody] AdminPasswordResetRequestDTOAPI request,
            CancellationToken cancellationToken)
        {
            var adminEmail = User.FindFirst(ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value;
            var adminPassword = request.AdminCurrentPassword ?? string.Empty;
            var targetEmail = request.TargetEmail?.Trim() ?? string.Empty;
            var newPassword = request.NewPassword ?? string.Empty;
            var confirmPassword = request.ConfirmPassword ?? string.Empty;

            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword) ||
                string.IsNullOrWhiteSpace(targetEmail) ||
                string.IsNullOrWhiteSpace(newPassword) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Preencha todos os campos."
                });
            }

            var admin = _userRepository.GetByEmail(adminEmail);
            if (admin is null ||
                !string.Equals(admin.Role, "ADMIN", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(admin.Password) ||
                !BCryptNet.Verify(adminPassword, admin.Password))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Não foi possível autorizar a operação administrativa."
                });
            }

            var targetUser = _userRepository.GetByEmail(targetEmail);
            if (targetUser is null ||
                string.IsNullOrWhiteSpace(targetUser.Id) ||
                IsProtectedAdminEmail(targetEmail) ||
                PasswordPolicyServiceAPI.IsProtectedAdministrator(targetUser) ||
                string.Equals(targetUser.Role, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    success = false,
                    message = GenericRecoveryError
                });
            }

            if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "A nova senha e a confirmação são diferentes."
                });
            }

            if (!PasswordPolicyServiceAPI.IsValid(newPassword))
            {
                return BadRequest(new
                {
                    success = false,
                    message = PasswordPolicyServiceAPI.GetValidationMessage()
                });
            }

            if (PasswordPolicyServiceAPI.MatchesCurrentOrHistory(newPassword, targetUser))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "A nova senha não pode ser igual à senha atual nem a uma das cinco senhas anteriores."
                });
            }

            var auditId = ObjectId.GenerateNewId();
            var auditFilter = Builders<BsonDocument>.Filter.Eq("_id", auditId);
            var auditDocument = new BsonDocument
            {
                ["_id"] = auditId,
                ["action"] = "ADMIN_PASSWORD_RESET",
                ["status"] = "REQUESTED",
                ["authorizedByUserId"] = admin.Id,
                ["authorizedByEmail"] = admin.Email ?? adminEmail,
                ["targetUserId"] = targetUser.Id,
                ["targetEmail"] = targetUser.Email ?? targetEmail,
                ["requestedAtUtc"] = new BsonDateTime(DateTime.UtcNow)
            };

            await _passwordAdministrationAudit.InsertOneAsync(
                auditDocument,
                cancellationToken: cancellationToken);

            PasswordPolicyServiceAPI.ApplyNewPassword(targetUser, newPassword);

            if (!_userRepository.Update(targetUser.Id, targetUser))
            {
                await _passwordAdministrationAudit.UpdateOneAsync(
                    auditFilter,
                    Builders<BsonDocument>.Update
                        .Set("status", "FAILED")
                        .Set("completedAtUtc", new BsonDateTime(DateTime.UtcNow)),
                    cancellationToken: cancellationToken);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Não foi possível salvar a nova senha."
                });
            }

            await _sessions.DeleteManyAsync(
                Builders<BsonDocument>.Filter.Eq("userId", targetUser.Id),
                cancellationToken);

            await _passwordAdministrationAudit.UpdateOneAsync(
                auditFilter,
                Builders<BsonDocument>.Update
                    .Set("status", "SUCCEEDED")
                    .Set("completedAtUtc", new BsonDateTime(DateTime.UtcNow)),
                cancellationToken: cancellationToken);

            return Ok(new
            {
                success = true,
                message = "Senha redefinida administrativamente. Todas as sessões foram encerradas."
            });
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public IActionResult Create([FromBody] UserDB user)
        {
            if (string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "E-mail e senha são obrigatórios."
                });
            }

            if (!PasswordPolicyServiceAPI.IsValid(user.Password))
            {
                return BadRequest(new
                {
                    success = false,
                    message =
                        PasswordPolicyServiceAPI.GetValidationMessage()
                });
            }

            if (_userRepository.GetByEmail(user.Email.Trim()) is not null)
            {
                return Conflict(new
                {
                    success = false,
                    message = "Já existe uma conta com esse e-mail."
                });
            }

            user.Email = user.Email.Trim();
            user.Role = "USER";
            user.CreatedAtUtc = DateTime.UtcNow;
            user.PasswordChangedAtUtc = DateTime.UtcNow;
            user.PasswordHistory = new List<string>();

            _userService.Create(user);

            return Ok(new
            {
                success = true,
                message = "Usuário criado com sucesso."
            });
        }

        [AllowAnonymous]
        [HttpPost("bootstrap-admin")]
        public IActionResult BootstrapAdmin(
            [FromBody] BootstrapAdminRequestDTOAPI request)
        {
            var configuredSecret = _configuration["BootstrapAdmin:Secret"];

            if (string.IsNullOrWhiteSpace(configuredSecret) ||
                Encoding.UTF8.GetByteCount(configuredSecret) < 32)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(request.BootstrapSecret))
            {
                return Unauthorized();
            }

            var expected = Encoding.UTF8.GetBytes(configuredSecret);
            var supplied = Encoding.UTF8.GetBytes(request.BootstrapSecret);

            if (expected.Length != supplied.Length ||
                !CryptographicOperations.FixedTimeEquals(
                    expected,
                    supplied))
            {
                return Unauthorized();
            }

            if (_userRepository.Get().Any(user =>
                string.Equals(
                    user.Role,
                    "ADMIN",
                    StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict(new
                {
                    message = "O administrador inicial já foi criado."
                });
            }

            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                !PasswordPolicyServiceAPI.IsValid(request.Password) ||
                request.Password.Length < 12)
            {
                return BadRequest(new
                {
                    message =
                        "Nome, e-mail e senha são obrigatórios. " +
                        "A senha deve ter pelo menos 12 caracteres e " +
                        "atender à política de complexidade."
                });
            }

            if (_userRepository.GetByEmail(request.Email.Trim()) is not null)
            {
                return Conflict(new
                {
                    message = "Já existe uma conta com esse e-mail."
                });
            }

            var admin = new UserDB
            {
                FullName = request.FullName.Trim(),
                Email = request.Email.Trim(),
                Password = request.Password,
                CPF = request.CPF,
                Login = string.IsNullOrWhiteSpace(request.Login)
                    ? request.Email.Trim()
                    : request.Login.Trim(),
                Role = "ADMIN",
                CreatedAtUtc = DateTime.UtcNow,
                PasswordChangedAtUtc = DateTime.UtcNow,
                PasswordHistory = new List<string>()
            };

            var lockDocument = new BsonDocument(
                "_id",
                "initial-admin-bootstrap");

            try
            {
                _bootstrapLocks.InsertOne(lockDocument);
            }
            catch (MongoWriteException exception)
                when (exception.WriteError?.Category ==
                      ServerErrorCategory.DuplicateKey)
            {
                return Conflict(new
                {
                    message =
                        "O bootstrap do administrador já foi utilizado " +
                        "ou está em andamento."
                });
            }

            try
            {
                _userService.Create(admin);
            }
            catch
            {
                _bootstrapLocks.DeleteOne(lockDocument);
                throw;
            }

            return Created(string.Empty, new
            {
                success = true,
                message =
                    "Administrador inicial criado. " +
                    "Remova/desative a configuração de bootstrap no ambiente."
            });
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Read(string id)
        {
            if (!IsCurrentUser(id))
            {
                return Forbid();
            }

            var user = _userService.Read(id);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.CPF,
                user.Email,
                user.Login
            });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] UserDB user)
        {
            if (!IsCurrentUser(id))
            {
                return Forbid();
            }

            var existingUser = _userService.Read(id);

            if (existingUser is null)
            {
                return NotFound();
            }

            user.Id = id;
            user.Role = existingUser.Role;
            user.CreatedAtUtc = existingUser.CreatedAtUtc;
            user.PasswordChangedAtUtc = existingUser.PasswordChangedAtUtc;
            user.PasswordHistory = existingUser.PasswordHistory;
            user.RecoveryFirstWindowStartedAtUtc =
                existingUser.RecoveryFirstWindowStartedAtUtc;
            user.RecoveryFirstWindowRequests =
                existingUser.RecoveryFirstWindowRequests;
            user.RecoverySecondWindowStartedAtUtc =
                existingUser.RecoverySecondWindowStartedAtUtc;
            user.RecoverySecondWindowRequests =
                existingUser.RecoverySecondWindowRequests;
            user.RecoverySuspendedUntilUtc =
                existingUser.RecoverySuspendedUntilUtc;
            user.RecoveryOffenses = existingUser.RecoveryOffenses;
            user.RecoveryLockedByAdmin = existingUser.RecoveryLockedByAdmin;
            user.RecoveryUnlockJustification =
                existingUser.RecoveryUnlockJustification;
            user.RecoveryUnlockedAtUtc = existingUser.RecoveryUnlockedAtUtc;
            user.RecoveryUnlockedByUserId =
                existingUser.RecoveryUnlockedByUserId;

            return _userService.Update(id, user);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (!IsCurrentUser(id))
            {
                return Forbid();
            }

            return _userService.Delete(id);
        }

        private IActionResult GenericRecoveryFailure()
        {
            return BadRequest(new
            {
                success = false,
                message = GenericRecoveryError
            });
        }

        private static string HashRecoveryCode(string code)
        {
            return Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(code)));
        }

        private static bool IsProtectedAdminEmail(string? email)
        {
            return string.Equals(
                email?.Trim(),
                ProtectedAdminEmail,
                StringComparison.OrdinalIgnoreCase);
        }

        private bool IsCurrentUser(string id)
        {
            return string.Equals(
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                id,
                StringComparison.Ordinal);
        }
    }

}
