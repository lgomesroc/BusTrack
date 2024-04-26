using AutoMapper;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.BusTrack.DB.ModelsDB;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class TripPassengerRepositoryDB : ITripPassengerRepositoryDB
    {
        private readonly IMongoCollection<TripPassengerDB> _tripPassengerCollection;
        private readonly IMapper _mapper;

        public TripPassengerRepositoryDB(IMongoDatabase database, IMapper mapper)
        {
            _tripPassengerCollection = database.GetCollection<TripPassengerDB>("TripsPassenger");
            _mapper = mapper;
        }
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
            var filter = Builders<TripPassengerDB>.Filter.Eq(t => t.TripId, id.ToString());
            return await _tripPassengerCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task AddTripsPassengerAsync(TripPassengerDB tripPassenger)
        {
            await _tripPassengerCollection.InsertOneAsync(tripPassenger);
        }

        public async Task UpdateTripsPassengerAsync(int id, TripPassengerDB tripPassenger)
        {
            var filter = Builders<TripPassengerDB>.Filter.Eq(t => t.TripId, id.ToString());
            await _tripPassengerCollection.ReplaceOneAsync(filter, tripPassenger);
        }

        public async Task DeleteTripsPassengerAsync(int id)
        {
            var filter = Builders<TripPassengerDB>.Filter.Eq(t => t.TripId, id.ToString());
            await _tripPassengerCollection.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<TripPassengerModelDB>> GetAllTripsPassengers()
        {
            var tripPassengers = await _tripPassengerCollection.Find(_ => true).ToListAsync();
            return _mapper.Map<IEnumerable<TripPassengerModelDB>>(tripPassengers);
        }

        public async Task<IEnumerable<TripPassengerDB>> GetAllTripPassengersAsync()
        {
            return await _tripPassengerCollection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<int>> GetTripIdsByPassengerIdAsync(int passengerId)
        {
            var filter = Builders<TripPassengerDB>.Filter.Eq(tp => tp.PassengerId, passengerId.ToString());
            var tripPassengers = await _tripPassengerCollection.Find(filter).ToListAsync();
            return tripPassengers.Select(tp => (Int32.Parse(tp.TripId)));
        }

        public async Task<IEnumerable<int>> GetPassengerIdsByTripIdAsync(int tripId)
        {
            var filter = Builders<TripPassengerDB>.Filter.Eq(tp => tp.TripId, tripId.ToString());
            var tripPassengers = await _tripPassengerCollection.Find(filter).ToListAsync();
            return tripPassengers.Select(tp => (Int32.Parse(tp.PassengerId)));
        }

        public async Task AddTripPassengerAsync(TripPassengerDB tripPassenger)
        {
            await _tripPassengerCollection.InsertOneAsync(tripPassenger);
        }

        public async Task RemoveTripPassengerAsync(int tripId, int passengerId)
        {
            var filter = Builders<TripPassengerDB>.Filter.And(
                Builders<TripPassengerDB>.Filter.Eq(tp => tp.TripId, tripId.ToString()),
                Builders<TripPassengerDB>.Filter.Eq(tp => tp.PassengerId, passengerId.ToString())
            );
            await _tripPassengerCollection.DeleteOneAsync(filter);
        }

        public async Task<TripPassengerModelDB> GetTripsPassengerById(int id)
        {
            var filter = Builders<TripPassengerDB>.Filter.Eq(tp => tp.TripId, id.ToString());
            var tripPassenger = await _tripPassengerCollection.Find(filter).FirstOrDefaultAsync();
            return _mapper.Map<TripPassengerModelDB>(tripPassenger);
        }

        public async Task<TripPassengerDB> CreateTripsPassenger(TripPassengerDB tripsPassenger)
        {
            await _tripPassengerCollection.InsertOneAsync(tripsPassenger);
            return tripsPassenger;
        }

        public async Task<TripPassengerDB> UpdateTripsPassenger(int id, TripPassengerDB tripPassenger)
        {
            var filter = Builders<TripPassengerDB>.Filter.Eq(tp => tp.TripId, id.ToString());
            await _tripPassengerCollection.ReplaceOneAsync(filter, tripPassenger);
            return tripPassenger;
        }

        public async Task<bool> DeleteTripsPassenger(int id)
        {
            var filter = Builders<TripPassengerDB>.Filter.Eq(tp => tp.TripId, id.ToString());
            var deleteResult = await _tripPassengerCollection.DeleteOneAsync(filter);
            return deleteResult.DeletedCount > 0;
        }
    }
}
