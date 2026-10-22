
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface IMaintenanceRepositoryDB
    {
        Task<IEnumerable<MaintenanceDB>> GetAllAsync();

        Task<MaintenanceDB?> GetByIdAsync(string id);

        Task<MaintenanceDB?> GetByIdIncludingDeletedAsync(string id);

        Task<MaintenanceDB> CreateAsync(MaintenanceDB maintenance);

        Task<bool> UpdateAsync(string id, MaintenanceDB maintenance);

        Task<bool> SoftDeleteAsync(string id, MaintenanceDB maintenance);
    }
}
