using System.Security.Claims;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.ClassesDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationControllerAPI : ControllerBase
    {
        private readonly IUserAuthenticationServiceAPI _userService;
        private readonly IJwtTokenServiceAPI _jwtTokenService;

        public AuthenticationControllerAPI(
            IUserAuthenticationServiceAPI userService,
            IJwtTokenServiceAPI jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
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

            user.Id = id;
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
