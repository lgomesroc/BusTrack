using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface ITripServiceAPI
    {
        Task<IEnumerable<TripDTOAPI>> GetAllTrips();
        Task<TripDTOAPI> GetTripById(int id);
        Task<TripDTOAPI> CreateTrip(TripDTOAPI trip);
        Task<TripDTOAPI> UpdateTrip(int id, TripDTOAPI trip);
        Task<bool> DeleteTrip(int id);
        Task<List<TripDB>> GetTrip();
    }
}
