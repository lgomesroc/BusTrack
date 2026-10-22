
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface IMaintenanceAuditRepositoryDB
    {
        Task<MaintenanceAuditDB> CreateAsync(
            MaintenanceAuditDB audit);

        Task<IEnumerable<MaintenanceAuditDB>> GetByMaintenanceIdAsync(
            string maintenanceId);
    }
}
