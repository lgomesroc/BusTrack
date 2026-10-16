using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.ClassesDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationControllerAPI : ControllerBase
    {
        private readonly IUserAuthenticationServiceAPI _userService;
        private readonly IJwtTokenServiceAPI _jwtTokenService;
        private readonly IUserRepositoryDB _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<BsonDocument> _bootstrapLocks;

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
            _bootstrapLocks = database.GetCollection<BsonDocument>("BusTrackSecurityLocks");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDTOAPI request)
        {
            var user = _userService.Authenticate(
                request.Email ?? string.Empty,
                request.Password ?? string.Empty);

            if (user == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "E-mail ou senha incorretos."
                });
            }

            var (accessToken, expiresAtUtc) = _jwtTokenService.CreateToken(user);

            return Ok(new AuthenticationResponseDTOAPI
            {
                Success = true,
                AccessToken = accessToken,
                ExpiresAtUtc = expiresAtUtc,
                WelcomeMessage = "Sucesso. Seja bem-vindo ao painel principal do Bus Track."
            });
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public IActionResult Create([FromBody] UserDB user)
        {
            user.Role = "USER";
            _userService.Create(user);

            return Ok(new
            {
                success = true,
                message = "Usuário criado com sucesso."
            });
        }

        [AllowAnonymous]
        [HttpPost("bootstrap-admin")]
        public IActionResult BootstrapAdmin([FromBody] BootstrapAdminRequestDTOAPI request)
        {
            var configuredSecret = _configuration["BootstrapAdmin:Secret"];
            if (string.IsNullOrWhiteSpace(configuredSecret) || Encoding.UTF8.GetByteCount(configuredSecret) < 32)
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
                !CryptographicOperations.FixedTimeEquals(expected, supplied))
            {
                return Unauthorized();
            }

            if (_userRepository.Get().Any(user =>
                string.Equals(user.Role, "ADMIN", StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict(new { message = "O administrador inicial já foi criado." });
            }

            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                request.Password.Length < 12)
            {
                return BadRequest(new
                {
                    message = "Nome, e-mail e senha são obrigatórios. A senha deve ter pelo menos 12 caracteres."
                });
            }

            var existingUser = _userRepository.GetByEmail(request.Email);
            if (existingUser is not null)
            {
                return Conflict(new { message = "Já existe uma conta com esse e-mail." });
            }

            var admin = new UserDB
            {
                FullName = request.FullName.Trim(),
                Email = request.Email.Trim(),
                Password = request.Password,
                CPF = request.CPF,
                Login = string.IsNullOrWhiteSpace(request.Login) ? request.Email.Trim() : request.Login.Trim(),
                Role = "ADMIN"
            };

            var lockDocument = new BsonDocument("_id", "initial-admin-bootstrap");
            try
            {
                _bootstrapLocks.InsertOne(lockDocument);
            }
            catch (MongoWriteException exception) when (exception.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                return Conflict(new { message = "O bootstrap do administrador já foi utilizado ou está em andamento." });
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
                message = "Administrador inicial criado. Remova/desative a configuração de bootstrap no ambiente."
            });
        }

        [HttpGet("{id}")]
        public IActionResult Read(string id)
        {
            if (!IsCurrentUser(id))
            {
                return Forbid();
            }

            var user = _userService.Read(id);
            if (user == null)
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

        private bool IsCurrentUser(string id) =>
            string.Equals(
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                id,
                StringComparison.Ordinal);
    }
}
