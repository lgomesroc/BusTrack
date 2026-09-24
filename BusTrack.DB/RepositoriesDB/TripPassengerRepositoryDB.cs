using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class TripPassengerRepositoryDB : ITripPassengerRepositoryDB
    {
        private readonly IMongoCollection<TripPassengerDB> _tripPassengerCollection;

        public TripPassengerRepositoryDB(IMongoDatabase database)
        {
            _tripPassengerCollection =
                database.GetCollection<TripPassengerDB>("TripsPassenger");
        }

        public async Task<IEnumerable<TripPassengerDB>> GetAllAsync()
        {
            return await _tripPassengerCollection
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<TripPassengerDB?> GetByIdAsync(string id)
        {
            var filter =
                Builders<TripPassengerDB>.Filter.Eq(
                    tripPassenger => tripPassenger.Id,
                    id);

            return await _tripPassengerCollection
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TripPassengerDB>> GetByTripIdAsync(
            string tripId)
        {
            var filter =
                Builders<TripPassengerDB>.Filter.Eq(
                    tripPassenger => tripPassenger.TripId,
                    tripId);

            return await _tripPassengerCollection
                .Find(filter)
                .ToListAsync();
        }

        public async Task<IEnumerable<TripPassengerDB>> GetByPassengerIdAsync(
            string passengerId)
        {
            var filter =
                Builders<TripPassengerDB>.Filter.Eq(
                    tripPassenger => tripPassenger.PassengerId,
                    passengerId);

            return await _tripPassengerCollection
                .Find(filter)
                .ToListAsync();
        }

        public async Task<TripPassengerDB> CreateAsync(
            TripPassengerDB tripPassenger)
        {
            await _tripPassengerCollection.InsertOneAsync(tripPassenger);

            return tripPassenger;
        }

        public async Task<TripPassengerDB?> UpdateAsync(
            string id,
            TripPassengerDB tripPassenger)
        {
            var filter =
                Builders<TripPassengerDB>.Filter.Eq(
                    existing => existing.Id,
                    id);

            var result =
                await _tripPassengerCollection.ReplaceOneAsync(
                    filter,
                    tripPassenger);

            if (result.MatchedCount == 0)
            {
                return null;
            }

            return tripPassenger;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var filter =
                Builders<TripPassengerDB>.Filter.Eq(
                    tripPassenger => tripPassenger.Id,
                    id);

            var result =
                await _tripPassengerCollection.DeleteOneAsync(filter);

            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteByTripAndPassengerAsync(
            string tripId,
            string passengerId)
        {
            var filter =
                Builders<TripPassengerDB>.Filter.And(
                    Builders<TripPassengerDB>.Filter.Eq(
                        tripPassenger => tripPassenger.TripId,
                        tripId),

                    Builders<TripPassengerDB>.Filter.Eq(
                        tripPassenger => tripPassenger.PassengerId,
                        passengerId));

            var result =
                await _tripPassengerCollection.DeleteOneAsync(filter);

            return result.DeletedCount > 0;
        }
    }
}
