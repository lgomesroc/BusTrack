using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface IBusRepositoryDB
    {
        Task<IEnumerable<BusDB>> GetAllBusesAsync();
        Task<BusDB> GetBusByIdAsync(string id);
        Task AddBusAsync(BusDB bus);
        Task UpdateBusAsync(string id, BusDB bus);
        Task DeleteBusAsync(string id);
    }
}
