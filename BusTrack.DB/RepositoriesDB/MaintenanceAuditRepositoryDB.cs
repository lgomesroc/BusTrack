
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class MaintenanceAuditRepositoryDB
        : IMaintenanceAuditRepositoryDB
    {
        private readonly IMongoCollection<MaintenanceAuditDB> _collection;

        public MaintenanceAuditRepositoryDB(IMongoDatabase database)
        {
            _collection =
                database.GetCollection<MaintenanceAuditDB>(
                    "MaintenanceAudits");
        }

        public async Task<MaintenanceAuditDB> CreateAsync(
            MaintenanceAuditDB audit)
        {
            if (!ObjectId.TryParse(audit.MaintenanceId, out _))
            {
                throw new ArgumentException(
                    "O identificador da manutenção é inválido.",
                    nameof(audit));
            }

            await _collection.InsertOneAsync(audit);

            return audit;
        }

        public async Task<IEnumerable<MaintenanceAuditDB>>
            GetByMaintenanceIdAsync(string maintenanceId)
        {
            if (!ObjectId.TryParse(maintenanceId, out _))
            {
                return [];
            }

            return await _collection
                .Find(audit => audit.MaintenanceId == maintenanceId)
                .SortByDescending(audit => audit.OccurredAtUtc)
                .ToListAsync();
        }
    }
}
