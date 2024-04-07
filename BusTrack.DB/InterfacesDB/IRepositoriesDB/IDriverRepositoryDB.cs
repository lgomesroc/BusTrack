using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface IDriverRepositoryDB
    {
        Task<IEnumerable<DriverDB>> GetAllDriversAsync();
        Task<DriverDB> GetDriverByIdAsync(string id);
        Task AddDriverAsync(DriverDB driver);
        Task UpdateDriverAsync(string id, DriverDB driver);
        Task DeleteDriverAsync(string id);
    }
}
