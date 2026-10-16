using System.Security.Claims;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ModelsAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Get()
        {
            var email = CurrentEmail();
            if (email is null)
            {
                return Unauthorized();
            }

            var account = _accountService.GetAccountByEmail(email);
            return account is null ? NotFound() : Ok(ToSafeAccount(account));
        }

        [HttpGet("{id}")]
        public IActionResult GetAccountById(string id)
        {
            var account = _accountService.GetAccountById(id);
            if (account is null)
            {
                return NotFound();
            }

            if (!IsOwner(account))
            {
                return Forbid();
            }

            return Ok(ToSafeAccount(account));
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateAccount([FromBody] AccountModelsAPI account)
        {
            _accountService.CreateAccount(account);
            return Ok(new { success = true, message = "Conta criada com sucesso." });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAccount(string id, [FromBody] AccountModelsAPI accountIn)
        {
            var existing = _accountService.GetAccountById(id);
            if (existing is null)
            {
                return NotFound();
            }

            if (!IsOwner(existing))
            {
                return Forbid();
            }

            accountIn.Id = id;
            accountIn.Email = existing.Email;
            accountIn.Password = existing.Password;

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
            var account = _accountService.GetAccountById(id);
            if (account is null)
            {
                return NotFound();
            }

            if (!IsOwner(account))
            {
                return Forbid();
            }

            var deleted = _accountService.DeleteAccount(id);
            if (!deleted)
            {
                return NotFound();
            }

            return Ok(new { success = true, message = "Conta excluída com sucesso." });
        }

        private string? CurrentEmail() =>
            User.FindFirst(ClaimTypes.Email)?.Value;

        private bool IsOwner(AccountModelsAPI account)
        {
            var email = CurrentEmail();
            return !string.IsNullOrWhiteSpace(email) &&
                string.Equals(account.Email, email, StringComparison.OrdinalIgnoreCase);
        }

        private static object ToSafeAccount(AccountModelsAPI account) => new
        {
            account.Id,
            account.FullName,
            account.CPF,
            account.Email
        };
    }
}
