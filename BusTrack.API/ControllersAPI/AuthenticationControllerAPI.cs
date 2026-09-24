using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.ClassesDB;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationControllerAPI : ControllerBase
    {
        private readonly IUserAuthenticationServiceAPI _userService;

        public AuthenticationControllerAPI(
            IUserAuthenticationServiceAPI userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDB request)
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

            return Ok(new
            {
                success = true,
                welcomeMessage =
                    "Sucesso. Seja bem-vindo ao painel principal do Bus Track."
            });
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] UserDB user)
        {
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
            var user = _userService.Read(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(
            string id,
            [FromBody] UserDB user)
        {
            return _userService.Update(id, user);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            return _userService.Delete(id);
        }
    }
}
