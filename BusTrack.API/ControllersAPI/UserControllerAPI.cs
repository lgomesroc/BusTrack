using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ModelsAPI;
using BusTrack.BusTrack.DB.ModelsDB;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BusTrack.BusTrack.API.ControllersAPI
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserControllerAPI : ControllerBase
    {
        private readonly IUserServiceAPI _userService;

        public UserControllerAPI(IUserServiceAPI userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserModelAPI user)
        {
            var userDb = new UserModelDB { /* mapeie os campos de UserModelsAPI para UserModelDB aqui */ };
            await _userService.CreateUser(userDb);
            return Ok("User created successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var usersDb = await _userService.GetAll();
            var usersApi = usersDb.Select(userDb => new UserModelAPI {  }).ToList();
            return Ok(usersApi);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var userDb = await _userService.GetById(id);
            if (userDb == null)
            {
                return NotFound();
            }
            var userApi = new UserModelAPI {  };
            return Ok(userApi);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UserModelAPI user)
        {
            var userDb = new UserModelDB { };
            await _userService.Update(id, userDb);
            return Ok("User updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _userService.Delete(id);
            return Ok("User deleted successfully.");
        }
    }
}

