using BusTrack.BusTrack.API.DTOAPI;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface ITripsPassengerServiceAPI
    {
        Task<IEnumerable<TripPassengerDTOAPI>> GetAllAsync();

        Task<TripPassengerDTOAPI?> GetByIdAsync(string id);

        Task<IEnumerable<TripPassengerDTOAPI>> GetByTripIdAsync(
            string? tripId);

        Task<IEnumerable<TripPassengerDTOAPI>> GetByPassengerIdAsync(
            string? passengerId);

        Task<TripPassengerDTOAPI> CreateAsync(
            TripPassengerDTOAPI tripPassenger);

        Task<TripPassengerDTOAPI?> UpdateAsync(
            string? id,
            TripPassengerDTOAPI tripPassenger);

        Task<bool> DeleteAsync(string id);

        Task<bool> DeleteByTripAndPassengerAsync(
            string? tripId,
            string? passengerId);
    }
}
