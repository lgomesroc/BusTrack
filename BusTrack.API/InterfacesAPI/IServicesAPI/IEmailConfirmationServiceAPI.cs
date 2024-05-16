using BusTrack.BusTrack.DB.ClassesDB;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface IEmailConfirmationServiceAPI
    {
        IActionResult Create(EmailConfirmationDB confirmation);
        EmailConfirmationDB Read(string id);
        IActionResult Update(string id, EmailConfirmationDB confirmation);
        IActionResult Delete(string id);
    }
}
