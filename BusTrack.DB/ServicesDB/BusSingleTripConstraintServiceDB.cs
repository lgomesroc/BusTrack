using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.ServicrDB
{
    public class BusSingleTripConstraintServiceDB
    {
        public static void ApplyBusSingleTripConstraint(IMongoDatabase database)
        {
            var tripsCollection = database.GetCollection<dynamic>("Trips");

            var indexKeysDefinition = Builders<dynamic>.IndexKeys.Ascending("busId");
            var uniqueIndexOptions = new CreateIndexOptions { Unique = true };

            tripsCollection.Indexes.CreateOne(new CreateIndexModel<dynamic>(indexKeysDefinition, uniqueIndexOptions));
        }
    }
}
