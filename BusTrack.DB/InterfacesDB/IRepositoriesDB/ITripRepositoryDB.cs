using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface ITripRepositoryDB
    {
        Task<IEnumerable<TripDB>> GetAllTripsAsync();
        Task<TripDB> GetTripByIdAsync(string id);
        Task AddTripAsync(TripDB trip);
        Task UpdateTripAsync(string id, TripDB trip);
        Task<bool> DeleteTripAsync(string id);
        Task<IEnumerable<TripDB>> GetAllTrips();
        Task<TripDB> GetTripById(int id);
    }
}
