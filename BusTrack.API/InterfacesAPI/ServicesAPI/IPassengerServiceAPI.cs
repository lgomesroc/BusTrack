using BusTrack.BusTrack.API.DTOAPI;

namespace BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI
{
    public interface IPassengerServiceAPI
    {
        Task<IEnumerable<PassengerDTOAPI>> GetAllPassengers();

        Task<PassengerDTOAPI> GetPassengerById(string id);
        Task<PassengerDTOAPI> CreatePassenger(PassengerDTOAPI passenger);
        Task<PassengerDTOAPI> UpdatePassenger(string id, PassengerDTOAPI passenger);
        Task<bool> DeletePassenger(string id);
    }
}
