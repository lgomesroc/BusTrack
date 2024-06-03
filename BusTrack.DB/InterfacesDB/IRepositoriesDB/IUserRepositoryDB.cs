using BusTrack.BusTrack.DB.ClassesDB;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface IUserRepositoryDB
    {
        List<UserDB> Get();

        UserDB GetByEmail(string email);

        UserDB Create(UserDB user);

        UserDB Read(string id);

        bool Update(string id, UserDB user);

        bool Delete(string id);
    }
}
