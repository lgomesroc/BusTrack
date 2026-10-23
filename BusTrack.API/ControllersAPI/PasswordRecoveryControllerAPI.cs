using System.Security.Claims;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.ClassesDB;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [ApiController]
    [Route("api/password-recovery")]
    public sealed class PasswordRecoveryControllerAPI : ControllerBase
    {
        private const int FirstWindowLimit = 3;
        private const int SecondWindowLimit = 5;

        private static readonly TimeSpan FirstWindowDuration =
            TimeSpan.FromDays(15);

        private static readonly TimeSpan SecondWindowDuration =
            TimeSpan.FromDays(21);

        private readonly IUserRepositoryDB _userRepository;

        public PasswordRecoveryControllerAPI(
            IUserRepositoryDB userRepository)
        {
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost("request")]
        public IActionResult RequestRecovery(
            [FromBody] PasswordRecoveryRequestDTOAPI request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Ok(new
                {
                    success = true,
                    message = "Se a conta estiver elegível, as instruções de recuperação serão disponibilizadas quando a verificação segura estiver implementada."
                });
            }

            var email = request.Email.Trim();
            var user = _userRepository.GetByEmail(email);

            // Resposta genérica para não revelar se a conta existe
            // nem se possui restrições especiais de recuperação.
            if (user is null ||
                PasswordPolicyServiceAPI.IsProtectedAdministrator(user))
            {
                return Ok(new
                {
                    success = true,
                    message = "Se a conta estiver elegível, as instruções de recuperação serão disponibilizadas quando a verificação segura estiver implementada."
                });
            }

            var now = DateTime.UtcNow;

            if (user.RecoveryLockedByAdmin)
            {
                return Ok(new
                {
                    success = true,
                    message = "A solicitação foi recebida. A recuperação de senha ainda depende da implementação da verificação segura."
                });
            }

            if (user.RecoverySuspendedUntilUtc.HasValue)
            {
                if (now < user.RecoverySuspendedUntilUtc.Value)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "A solicitação foi recebida. A recuperação de senha ainda depende da implementação da verificação segura."
                    });
                }

                // A suspensão terminou. As janelas são reiniciadas,
                // mas o histórico de infrações é preservado.
                user.RecoverySuspendedUntilUtc = null;
                user.RecoveryFirstWindowStartedAtUtc = null;
                user.RecoveryFirstWindowRequests = 0;
                user.RecoverySecondWindowStartedAtUtc = null;
                user.RecoverySecondWindowRequests = 0;
            }

            // Primeira janela: até 3 solicitações em 15 dias.
            if (user.RecoveryFirstWindowRequests < FirstWindowLimit &&
                (user.RecoveryFirstWindowStartedAtUtc is null ||
                 now - user.RecoveryFirstWindowStartedAtUtc.Value >=
                 FirstWindowDuration))
            {
                user.RecoveryFirstWindowStartedAtUtc = now;
                user.RecoveryFirstWindowRequests = 0;
            }

            if (user.RecoveryFirstWindowRequests < FirstWindowLimit)
            {
                user.RecoveryFirstWindowRequests++;
            }
            else
            {
                // Segunda janela: começa quando a primeira cota
                // já foi esgotada. São 5 solicitações em 21 dias.
                if (user.RecoverySecondWindowStartedAtUtc is null)
                {
                    user.RecoverySecondWindowStartedAtUtc = now;
                    user.RecoverySecondWindowRequests = 0;
                }

                var secondWindowExpired =
                    now - user.RecoverySecondWindowStartedAtUtc.Value >=
                    SecondWindowDuration;

                if (secondWindowExpired)
                {
                    ApplyRecoveryOffense(user, now);
                }
                else if (user.RecoverySecondWindowRequests <
                         SecondWindowLimit)
                {
                    user.RecoverySecondWindowRequests++;
                }
                else
                {
                    ApplyRecoveryOffense(user, now);
                }
            }

            _userRepository.Update(user.Id!, user);

            return Ok(new
            {
                success = true,
                message = "A solicitação foi recebida. A recuperação de senha ainda depende da implementação da verificação segura."
            });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost("{userId}/unlock")]
        public IActionResult UnlockRecovery(
            string userId,
            [FromBody] UnlockPasswordRecoveryRequestDTOAPI request)
        {
            if (string.IsNullOrWhiteSpace(request.Justification) ||
                request.Justification.Trim().Length < 10)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Informe uma justificativa com pelo menos 10 caracteres."
                });
            }

            var user = _userRepository.Read(userId);

            if (user is null ||
                PasswordPolicyServiceAPI.IsProtectedAdministrator(user))
            {
                return NotFound(new
                {
                    success = false,
                    message = "Não foi possível realizar a operação para essa conta."
                });
            }

            var now = DateTime.UtcNow;

            var adminId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? "unknown";

            user.RecoveryLockedByAdmin = false;
            user.RecoverySuspendedUntilUtc = null;
            user.RecoveryOffenses = 0;

            user.RecoveryFirstWindowStartedAtUtc = null;
            user.RecoveryFirstWindowRequests = 0;
            user.RecoverySecondWindowStartedAtUtc = null;
            user.RecoverySecondWindowRequests = 0;

            user.RecoveryUnlockJustification =
                request.Justification.Trim();

            user.RecoveryUnlockedAtUtc = now;
            user.RecoveryUnlockedByUserId = adminId;

            var updated = _userRepository.Update(userId, user);

            if (!updated)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = "Não foi possível salvar a liberação."
                    });
            }

            return Ok(new
            {
                success = true,
                message = "A recuperação da conta foi liberada."
            });
        }

        private static void ApplyRecoveryOffense(
            UserDB user,
            DateTime now)
        {
            user.RecoveryOffenses++;

            user.RecoveryFirstWindowStartedAtUtc = null;
            user.RecoveryFirstWindowRequests = 0;
            user.RecoverySecondWindowStartedAtUtc = null;
            user.RecoverySecondWindowRequests = 0;

            switch (user.RecoveryOffenses)
            {
                case 1:
                    user.RecoverySuspendedUntilUtc = now.AddDays(15);
                    break;

                case 2:
                    user.RecoverySuspendedUntilUtc = now.AddDays(30);
                    break;

                default:
                    user.RecoverySuspendedUntilUtc = null;
                    user.RecoveryLockedByAdmin = true;
                    break;
            }
        }
    }
}
