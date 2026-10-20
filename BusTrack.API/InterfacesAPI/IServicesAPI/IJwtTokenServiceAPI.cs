using BusTrack.BusTrack.DB.ClassesDB;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface IJwtTokenServiceAPI
    {
        (string AccessToken, DateTime ExpiresAtUtc) CreateToken(
            UserDB user,
            string browserId,
            string sessionId);
    }
}