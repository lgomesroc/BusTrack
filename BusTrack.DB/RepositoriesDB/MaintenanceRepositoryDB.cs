
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class MaintenanceRepositoryDB : IMaintenanceRepositoryDB
    {
        private readonly IMongoCollection<MaintenanceDB> _collection;

        public MaintenanceRepositoryDB(IMongoDatabase database)
        {
            _collection = database.GetCollection<MaintenanceDB>(
                "Maintenances");
        }

        public async Task<IEnumerable<MaintenanceDB>> GetAllAsync()
        {
            var filter = Builders<MaintenanceDB>.Filter.Ne(
                item => item.IsDeleted,
                true);

            return await _collection
                .Find(filter)
                .SortByDescending(item => item.ServiceDate)
                .ToListAsync();
        }

        public async Task<MaintenanceDB?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            var filter = Builders<MaintenanceDB>.Filter.And(
                Builders<MaintenanceDB>.Filter.Eq(item => item.Id, id),
                Builders<MaintenanceDB>.Filter.Ne(item => item.IsDeleted, true));

            return await _collection
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<MaintenanceDB?> GetByIdIncludingDeletedAsync(
            string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            return await _collection
                .Find(item => item.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<MaintenanceDB> CreateAsync(
            MaintenanceDB maintenance)
        {
            await _collection.InsertOneAsync(maintenance);
            return maintenance;
        }

        public async Task<bool> UpdateAsync(
            string id,
            MaintenanceDB maintenance)
        {
            if (!ObjectId.TryParse(id, out _) || maintenance.IsDeleted)
            {
                return false;
            }

            maintenance.Id = id;

            var filter = Builders<MaintenanceDB>.Filter.And(
                Builders<MaintenanceDB>.Filter.Eq(item => item.Id, id),
                Builders<MaintenanceDB>.Filter.Ne(item => item.IsDeleted, true));

            var result = await _collection.ReplaceOneAsync(
                filter,
                maintenance);

            return result.MatchedCount > 0;
        }

        public async Task<bool> SoftDeleteAsync(
            string id,
            MaintenanceDB maintenance)
        {
            if (!ObjectId.TryParse(id, out _) ||
                !maintenance.IsDeleted ||
                string.IsNullOrWhiteSpace(maintenance.DeletionReason))
            {
                return false;
            }

            maintenance.Id = id;

            var filter = Builders<MaintenanceDB>.Filter.And(
                Builders<MaintenanceDB>.Filter.Eq(item => item.Id, id),
                Builders<MaintenanceDB>.Filter.Ne(item => item.IsDeleted, true));

            var result = await _collection.ReplaceOneAsync(
                filter,
                maintenance);

            return result.MatchedCount > 0;
        }
    }
}
