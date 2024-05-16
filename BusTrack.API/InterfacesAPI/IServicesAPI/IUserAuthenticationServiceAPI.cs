using BusTrack.BusTrack.DB.ClassesDB;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface IUserAuthenticationServiceAPI
    {
        UserDB Authenticate(string email, string password);
        IActionResult Create(UserDB user);
        UserDB Read(string id);
        IActionResult Update(string id, UserDB user);
        IActionResult Delete(string id);
    }
}
