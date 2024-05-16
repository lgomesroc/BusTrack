using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class PassengerRepositoryDB : IPassengerRepositoryDB
    {
        private readonly IMongoCollection<PassengerDB> _passengersCollection;

        public PassengerRepositoryDB(IMongoDatabase database)
        {
            _passengersCollection = database.GetCollection<PassengerDB>("Passengers");
        }

        public async Task<IEnumerable<PassengerDB>> GetAllPassengersAsync()
        {
            return await _passengersCollection.Find(_ => true).ToListAsync();
        }

        public async Task<PassengerDB> GetPassengerByIdAsync(string id)
        {
            var filter = Builders<PassengerDB>.Filter.Eq(p => p.Id, id);
            return await _passengersCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task AddPassengerAsync(PassengerDB passenger)
        {
            await _passengersCollection.InsertOneAsync(passenger);
        }

        public async Task UpdatePassengerAsync(string id, PassengerDB passenger)
        {
            var filter = Builders<PassengerDB>.Filter.Eq(p => p.Id, id);
            await _passengersCollection.ReplaceOneAsync(filter, passenger);
        }

        public async Task DeletePassengerAsync(string id)
        {
            var filter = Builders<PassengerDB>.Filter.Eq(p => p.Id, id);
            await _passengersCollection.DeleteOneAsync(filter);
        }

        public async Task<bool> DeletePassenger(string id)
        {
            var filter = Builders<PassengerDB>.Filter.Eq(p => p.Id, id);
            var deleteResult = await _passengersCollection.DeleteOneAsync(filter);
            return deleteResult.DeletedCount > 0;
        }

    }
}
