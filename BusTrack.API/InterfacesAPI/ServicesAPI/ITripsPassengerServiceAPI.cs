using BusTrack.BusTrack.API.DTOAPI;

namespace BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI
{
    public interface ITripsPassengerServiceAPI
    {
        Task<bool> DeleteTripsPassenger(int id);
        Task<IEnumerable<TripPassengerDTOAPI>> GetAllTripsPassengers();
        Task<TripPassengerDTOAPI> GetTripsPassengerById(int id);
        Task<TripPassengerDTOAPI> CreateTripsPassenger(TripPassengerDTOAPI tripsPassenger);
        Task<TripPassengerDTOAPI> UpdateTripsPassenger(int id, TripPassengerDTOAPI tripsPassenger);

    }
}
