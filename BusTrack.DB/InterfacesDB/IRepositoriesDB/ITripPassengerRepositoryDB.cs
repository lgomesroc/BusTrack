using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.ModelsDB;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface ITripPassengerRepositoryDB
    {
        Task<IEnumerable<TripPassengerDB>> GetAllTripPassengersAsync();
        Task<IEnumerable<int>> GetPassengerIdsByTripIdAsync(int tripId);
        Task<IEnumerable<int>> GetTripIdsByPassengerIdAsync(int passengerId);
        Task AddTripPassengerAsync(TripPassengerDB tripPassenger);
        Task RemoveTripPassengerAsync(int tripId, int passengerId);
        Task<IEnumerable<TripPassengerModelDB>> GetAllTripsPassengers();
        Task<TripPassengerModelDB> GetTripsPassengerById(int id);
        Task<TripPassengerDB> CreateTripsPassenger(TripPassengerDB tripsPassenger);
        Task<TripPassengerDB> UpdateTripsPassenger(int id, TripPassengerDB tripPassenger);
        Task<bool> DeleteTripsPassenger(int id);



    }
}