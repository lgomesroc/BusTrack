using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class MaintenanceRepositoryDB : IMaintenanceRepositoryDB
    {
        private readonly IMongoCollection<MaintenanceDB> _collection;

        public MaintenanceRepositoryDB(IMongoDatabase database)
        {
            _collection = database.GetCollection<MaintenanceDB>("Maintenances");
        }

        public async Task<IEnumerable<MaintenanceDB>> GetAllAsync()
        {
            return await _collection.Find(_ => true)
                .SortByDescending(item => item.ServiceDate)
                .ToListAsync();
        }

        public async Task<MaintenanceDB?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            return await _collection.Find(item => item.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<MaintenanceDB> CreateAsync(MaintenanceDB maintenance)
        {
            await _collection.InsertOneAsync(maintenance);
            return maintenance;
        }

        public async Task<bool> UpdateAsync(string id, MaintenanceDB maintenance)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return false;
            }

            maintenance.Id = id;
            var result = await _collection.ReplaceOneAsync(item => item.Id == id, maintenance);
            return result.MatchedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return false;
            }

            var result = await _collection.DeleteOneAsync(item => item.Id == id);
            return result.DeletedCount > 0;
        }
    }
}