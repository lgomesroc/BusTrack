using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class BusRepositoryDB : IBusRepositoryDB
    {
        private readonly IMongoCollection<BusDB> _busesCollection;

        public BusRepositoryDB(IMongoDatabase database)
        {
            _busesCollection = database.GetCollection<BusDB>("Buses");
        }

        public async Task<IEnumerable<BusDB>> GetAllBusesAsync()
        {
            return await _busesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<BusDB> GetBusByIdAsync(string id)
        {
            var filter = Builders<BusDB>.Filter.Eq(b => b.Id, id);
            return await _busesCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task AddBusAsync(BusDB bus)
        {
            await _busesCollection.InsertOneAsync(bus);
        }

        public async Task UpdateBusAsync(string id, BusDB bus)
        {
            var filter = Builders<BusDB>.Filter.Eq(b => b.Id, id);
            await _busesCollection.ReplaceOneAsync(filter, bus);
        }

        public async Task DeleteBusAsync(string id)
        {
            var filter = Builders<BusDB>.Filter.Eq(b => b.Id, id);
            await _busesCollection.DeleteOneAsync(filter);
        }
    }
}