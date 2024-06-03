using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface IPassengerServiceAPI
    {
        Task<IEnumerable<PassengerDTOAPI>> GetAllPassengers();
        Task<PassengerDTOAPI> GetPassengerById(string id);
        Task<PassengerDTOAPI> CreatePassenger(PassengerDTOAPI passenger);
        Task<PassengerDTOAPI> UpdatePassenger(string id, PassengerDTOAPI passenger);
        Task<bool> DeletePassenger(string id);
        Task<List<PassengerDB>> GetPassengers();
        Task AddPassengerAsync(PassengerDTOAPI passengerDto);
        Task<PassengerDTOAPI> GetPassengerByIdAsync(int id);
        Task UpdatePassengerAsync(PassengerDTOAPI passengerDto);
        Task DeletePassengerAsync(int id);
        Task<IEnumerable<PassengerDTOAPI>> GetAllPassengersAsync();
    }
}
