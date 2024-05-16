using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ModelsAPI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [ApiController]
    [Route("[controller]")]
    public class AccountControllerAPI : ControllerBase
    {
        private readonly IAccountServiceAPI _accountService;

        public AccountControllerAPI(IAccountServiceAPI accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Get() =>
            _accountService.Get();

        [HttpGet("{id}")]
        public IActionResult GetAccountById(string id)
        {
            var account = _accountService.GetAccountById(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        [HttpPost]
        public IActionResult CreateAccount([FromBody] AccountModelsAPI account)
        {
            _accountService.CreateAccount(account);
            return Ok(new { success = true, message = "Conta criada com sucesso." });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAccount(string id, [FromBody] AccountModelsAPI accountIn)
        {
            var updated = _accountService.UpdateAccount(id, accountIn);
            if (!updated)
            {
                return NotFound();
            }
            return Ok(new { success = true, message = "Conta atualizada com sucesso." });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAccount(string id)
        {
            var deleted = _accountService.DeleteAccount(id);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok(new { success = true, message = "Conta excluída com sucesso." });
        }
    }
}
