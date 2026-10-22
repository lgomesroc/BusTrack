
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface IMaintenanceServiceAPI
    {
        Task<IEnumerable<MaintenanceDTOAPI>> GetAllAsync();

        Task<MaintenanceDTOAPI?> GetByIdAsync(string id);

        Task<IEnumerable<MaintenanceAuditDB>?> GetAuditAsync(string id);

        Task<MaintenanceDTOAPI?> CreateAsync(
            MaintenanceDTOAPI maintenance);

        Task<MaintenanceDTOAPI?> UpdateAsync(
            string id,
            MaintenanceDTOAPI maintenance);

        Task<bool> DeleteAsync(string id, string reason);
    }
}
