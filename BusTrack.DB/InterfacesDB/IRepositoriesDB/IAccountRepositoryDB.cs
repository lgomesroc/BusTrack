using BusTrack.BusTrack.DB.ModelsDB;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface IAccountRepositoryDB
    {
        List<AccountModelDB> Get();

        AccountModelDB? GetByEmail(string email);

        AccountModelDB Create(AccountModelDB account);

        AccountModelDB? Read(string id);

        bool Update(string id, AccountModelDB account);

        bool Delete(string id);
    }
}
