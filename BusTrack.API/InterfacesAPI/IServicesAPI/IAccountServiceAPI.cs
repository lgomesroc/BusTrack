using BusTrack.BusTrack.API.ModelsAPI;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface IAccountServiceAPI
    {
        IActionResult Get();
        AccountModelsAPI GetAccountById(string id);
        void CreateAccount(AccountModelsAPI account);
        IActionResult GetAllAccounts();
        bool UpdateAccount(string id, AccountModelsAPI accountIn);
        bool DeleteAccount(string id);
    }
}
