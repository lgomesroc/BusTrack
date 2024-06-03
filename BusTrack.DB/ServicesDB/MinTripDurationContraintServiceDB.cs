using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.ServicrDB
{
    public class MinTripDurationContraintServiceDB
    {
        public static void ApplyMinTripDurationConstraint(IMongoDatabase database)
        {
            var tripsCollection = database.GetCollection<BsonDocument>("Trips");

            var filter = Builders<BsonDocument>.Filter.Empty;
            var update = Builders<BsonDocument>.Update.Inc("arrivalTime", TimeSpan.FromHours(1));

            tripsCollection.UpdateMany(filter, update);
        }
    }

}
