using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface ITripPassengerRepositoryDB
    {
        Task<IEnumerable<TripPassengerDB>> GetAllAsync();

        Task<TripPassengerDB?> GetByIdAsync(string id);

        Task<IEnumerable<TripPassengerDB>> GetByTripIdAsync(string tripId);

        Task<IEnumerable<TripPassengerDB>> GetByPassengerIdAsync(string passengerId);

        Task<TripPassengerDB> CreateAsync(TripPassengerDB tripPassenger);

        Task<TripPassengerDB?> UpdateAsync(
            string id,
            TripPassengerDB tripPassenger);

        Task<bool> DeleteAsync(string id);

        Task<bool> DeleteByTripAndPassengerAsync(
            string tripId,
            string passengerId);
    }
}
