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

        public AuthenticationControllerAPI(IUserAuthenticationServiceAPI userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDB request)
        {
            var user = _userService.Authenticate(request.Email, request.Password);

            if (user == null)
                return Unauthorized(new { message = "E-mail ou senha incorretos" });

            // Aqui você pode gerar o token JWT ou criar uma sessão de usuário, etc.

            return Ok(new { message = "Login bem-sucedido" });
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] UserDB user)
        {
            _userService.Create(user);
            return Ok(new { message = "Usuário criado com sucesso" });
        }

        [HttpGet("{id}")]
        public IActionResult Read(string id)
        {
            var user = _userService.Read(id);
            return Ok(user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] UserDB user)
        {
            _userService.Update(id, user);
            return Ok(new { message = "Usuário atualizado com sucesso" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _userService.Delete(id);
            return Ok(new { message = "Usuário deletado com sucesso" });
        }
    }

}
