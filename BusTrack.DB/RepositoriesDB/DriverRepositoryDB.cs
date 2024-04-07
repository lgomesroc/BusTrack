using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class DriverRepositoryDB : IDriverRepositoryDB
    {
        private readonly IMongoCollection<DriverDB> _driversCollection;

        public DriverRepositoryDB(IMongoDatabase database)
        {
            _driversCollection = database.GetCollection<DriverDB>("Drivers");
        }

        public async Task<IEnumerable<DriverDB>> GetAllDriversAsync()
        {
            return await _driversCollection.Find(_ => true).ToListAsync();
        }

        public async Task<DriverDB> GetDriverByIdAsync(string id)
        {
            var filter = Builders<DriverDB>.Filter.Eq(d => d.Id, id);
            return await _driversCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task AddDriverAsync(DriverDB driver)
        {
            await _driversCollection.InsertOneAsync(driver);
        }

        public async Task UpdateDriverAsync(string id, DriverDB driver)
        {
            var filter = Builders<DriverDB>.Filter.Eq(d => d.Id, id);
            await _driversCollection.ReplaceOneAsync(filter, driver);
        }

        public async Task DeleteDriverAsync(string id)
        {
            var filter = Builders<DriverDB>.Filter.Eq(d => d.Id, id);
            await _driversCollection.DeleteOneAsync(filter);
        }
    }
}
