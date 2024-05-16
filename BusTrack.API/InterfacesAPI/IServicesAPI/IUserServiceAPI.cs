using BusTrack.BusTrack.DB.ModelsDB;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface IUserServiceAPI
    {
        Task<UserModelDB> Authenticate(string email, string password);
        Task CreateUser(UserModelDB user);
        Task<List<UserModelDB>> GetAll();
        Task<UserModelDB> GetById(string id);
        Task Update(string id, UserModelDB user);
        Task Delete(string id);
    }
}
