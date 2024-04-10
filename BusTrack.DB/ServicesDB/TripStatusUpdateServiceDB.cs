using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.ServicrDB
{
    public class TripStatusUpdateServiceDB
    {
        public static void UpdateTripStatus(IMongoDatabase database, ObjectId tripId)
        {
            var tripsCollection = database.GetCollection<dynamic>("Trips");

            var filter = Builders<dynamic>.Filter.Eq("_id", tripId);
            var update = Builders<dynamic>.Update.Set("status", "Concluída");

            tripsCollection.UpdateOne(filter, update);
        }
    }
}
