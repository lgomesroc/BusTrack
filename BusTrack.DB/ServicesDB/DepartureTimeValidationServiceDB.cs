using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.ServicrDB
{
    public class DepartureTimeValidationServiceDB
    {
        public static void ApplyDepartureTimeValidation(IMongoDatabase database)
        {
            var tripsCollection = database.GetCollection<dynamic>("Trips");

            var filter = Builders<dynamic>.Filter.Lt("departureTime", DateTime.Now);
            var update = Builders<dynamic>.Update.Set("departureTime", DateTime.Now);

            tripsCollection.UpdateMany(filter, update);
        }
    }
}
