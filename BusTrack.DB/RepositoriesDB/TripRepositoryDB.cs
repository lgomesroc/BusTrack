using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class TripRepositoryDB : ITripRepositoryDB
    {
        private readonly IMongoCollection<TripDB> _tripsCollection;

        public TripRepositoryDB(
            IMongoDatabase database)
        {
            _tripsCollection =
                database.GetCollection<TripDB>("Trips");
        }

        public async Task<IEnumerable<TripDB>> GetAllTripsAsync()
        {
            return await _tripsCollection
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<TripDB?> GetTripByIdAsync(
            string id)
        {
            var filter =
                Builders<TripDB>.Filter.Eq(
                    trip => trip.Id,
                    id);

            return await _tripsCollection
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<TripDB> AddTripAsync(
            TripDB trip)
        {
            await _tripsCollection
                .InsertOneAsync(trip);

            return trip;
        }

        public async Task<TripDB?> UpdateTripAsync(
            string id,
            TripDB trip)
        {
            var filter =
                Builders<TripDB>.Filter.Eq(
                    existingTrip => existingTrip.Id,
                    id);

            var result =
                await _tripsCollection.ReplaceOneAsync(
                    filter,
                    trip);

            if (result.MatchedCount == 0)
            {
                return null;
            }

            return trip;
        }

        public async Task<bool> DeleteTripAsync(
            string id)
        {
            var filter =
                Builders<TripDB>.Filter.Eq(
                    trip => trip.Id,
                    id);

            var result =
                await _tripsCollection.DeleteOneAsync(
                    filter);

            return result.DeletedCount > 0;
        }
    }
}
