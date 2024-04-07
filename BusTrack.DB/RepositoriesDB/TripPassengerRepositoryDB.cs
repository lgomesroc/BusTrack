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
            _tripPassengerCollection = database.GetCollection<TripPassengerDB>("TripsPassenger");
        }

        public async Task<IEnumerable<TripPassengerDB>> GetAllTripsPassengerAsync()
        {
            return await _tripPassengerCollection.Find(_ => true).ToListAsync();
        }

        public async Task<TripPassengerDB> GetTripsPassengerByIdAsync(int id)
        {
            var filter = Builders<TripPassengerDB>.Filter.Eq(t => t.TripId, id);
            return await _tripPassengerCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task AddTripsPassengerAsync(TripPassengerDB tripPassenger)
        {
            await _tripPassengerCollection.InsertOneAsync(tripPassenger);
        }

        public async Task UpdateTripsPassengerAsync(int id, TripPassengerDB tripPassenger)
        {
            var filter = Builders<TripPassengerDB>.Filter.Eq(t => t.TripId, id);
            await _tripPassengerCollection.ReplaceOneAsync(filter, tripPassenger);
        }

        public async Task DeleteTripsPassengerAsync(int id)
        {
            var filter = Builders<TripPassengerDB>.Filter.Eq(t => t.TripId, id);
            await _tripPassengerCollection.DeleteOneAsync(filter);
        }

        public Task<IEnumerable<TripPassengerDB>> GetAllTripPassengersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<int>> GetPassengerIdsByTripIdAsync(int tripId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<int>> GetTripIdsByPassengerIdAsync(int passengerId)
        {
            throw new NotImplementedException();
        }

        public Task AddTripPassengerAsync(TripPassengerDB tripPassenger)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTripPassengerAsync(int tripId, int passengerId)
        {
            throw new NotImplementedException();
        }
    }
}
