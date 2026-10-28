using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface ITripAuditRepositoryDB
    {
        Task<TripAuditDB> CreateAsync(TripAuditDB audit);
        Task<IEnumerable<TripAuditDB>> GetByTripIdAsync(string tripId);
    }
}