using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI
{
    public interface ITripsPassengerServiceAPI
    {
        Task<bool> DeleteTripsPassenger(string id);
        Task<IEnumerable<TripPassengerDTOAPI>> GetAllTripsPassengers();
        Task<TripPassengerDTOAPI> GetTripsPassengerById(string id);
        Task<TripPassengerDTOAPI> CreateTripsPassenger(TripPassengerDTOAPI tripsPassenger);
        Task<TripPassengerDTOAPI> UpdateTripsPassenger(string id, TripPassengerDTOAPI tripsPassenger);
        Task<List<TripPassengerDB>> GetTripsPassengers();
    }
}
