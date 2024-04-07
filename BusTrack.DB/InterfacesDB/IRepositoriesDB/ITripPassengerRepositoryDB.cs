using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface ITripPassengerRepositoryDB
    {
        Task<IEnumerable<TripPassengerDB>> GetAllTripPassengersAsync();
        Task<IEnumerable<int>> GetPassengerIdsByTripIdAsync(int tripId);
        Task<IEnumerable<int>> GetTripIdsByPassengerIdAsync(int passengerId);
        Task AddTripPassengerAsync(TripPassengerDB tripPassenger);
        Task RemoveTripPassengerAsync(int tripId, int passengerId);
    }
}