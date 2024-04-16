using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class TripRepositoryDB : ITripRepositoryDB
    {
        private readonly IMongoCollection<TripDB> _tripsCollection;

        public TripRepositoryDB(IMongoDatabase database)
        {
            _tripsCollection = database.GetCollection<TripDB>("Trips");
        }

        public async Task<IEnumerable<TripDB>> GetAllTripsAsync()
        {
            return await _tripsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<TripDB> GetTripByIdAsync(string id)
        {
            var filter = Builders<TripDB>.Filter.Eq(t => t.Id, id);
            return await _tripsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task AddTripAsync(TripDB trip)
        {
            await _tripsCollection.InsertOneAsync(trip);
        }

        public async Task UpdateTripAsync(string id, TripDB trip)
        {
            var filter = Builders<TripDB>.Filter.Eq(t => t.Id, id);
            await _tripsCollection.ReplaceOneAsync(filter, trip);
        }

        public async Task<bool> DeleteTripAsync(string id)
        {
            var filter = Builders<TripDB>.Filter.Eq(t => t.Id, id);
            var result = await _tripsCollection.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }

        public async Task<TripDB> GetTripById(int id)
        {
            var filter = Builders<TripDB>.Filter.Eq(t => t.Id, id.ToString());
            return await _tripsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TripDB>> GetAllTrips()
        {
            return await _tripsCollection.Find(_ => true).ToListAsync();
        }
    }
}
