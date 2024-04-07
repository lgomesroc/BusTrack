using BusTrack.BusTrack.DB.Classes;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.ServicrDB
{
    public class TripServiceDB
    {
        private readonly IMongoCollection<TripDB> _tripsCollection;

        public TripServiceDB(IMongoDatabase database)
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

        public async Task DeleteTripAsync(string id)
        {
            var filter = Builders<TripDB>.Filter.Eq(t => t.Id, id);
            await _tripsCollection.DeleteOneAsync(filter);
        }
    }
}
