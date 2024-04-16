using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface IPassengerRepositoryDB
    {
        Task<IEnumerable<PassengerDB>> GetAllPassengersAsync();
        Task<PassengerDB> GetPassengerByIdAsync(string id);
        Task AddPassengerAsync(PassengerDB passenger);
        Task UpdatePassengerAsync(string id, PassengerDB passenger);
        Task DeletePassengerAsync(string id);
        Task<bool> DeletePassenger(string id);
    }
}
