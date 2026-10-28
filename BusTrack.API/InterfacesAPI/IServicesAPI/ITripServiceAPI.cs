using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface ITripServiceAPI
    {
        Task<IEnumerable<TripDTOAPI>> GetAllTripsAsync();
        Task<TripDTOAPI?> GetTripByIdAsync(string? id);
        Task<IEnumerable<TripDetailsDTOAPI>> GetAllTripDetailsAsync();
        Task<TripDetailsDTOAPI?> GetTripDetailsByIdAsync(string? id);
        Task<TripDTOAPI> CreateTripAsync(TripDTOAPI trip);
        Task<TripDTOAPI?> UpdateTripAsync(string? id, TripDTOAPI trip);
        Task<bool> DeleteTripAsync(string? id);
        Task<IEnumerable<TripAuditDB>> GetAuditAsync(string? id);
    }
}