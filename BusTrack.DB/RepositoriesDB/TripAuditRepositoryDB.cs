using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class TripAuditRepositoryDB : ITripAuditRepositoryDB
    {
        private readonly IMongoCollection<TripAuditDB> _collection;

        public TripAuditRepositoryDB(IMongoDatabase database)
        {
            _collection = database.GetCollection<TripAuditDB>("TripAudits");
        }

        public async Task<TripAuditDB> CreateAsync(TripAuditDB audit)
        {
            if (!ObjectId.TryParse(audit.TripId, out _))
                throw new ArgumentException("O identificador da viagem é inválido.", nameof(audit));

            await _collection.InsertOneAsync(audit);
            return audit;
        }

        public async Task<IEnumerable<TripAuditDB>> GetByTripIdAsync(string tripId)
        {
            if (!ObjectId.TryParse(tripId, out _)) return [];

            return await _collection
                .Find(audit => audit.TripId == tripId)
                .SortByDescending(audit => audit.OccurredAtUtc)
                .ToListAsync();
        }
    }
}