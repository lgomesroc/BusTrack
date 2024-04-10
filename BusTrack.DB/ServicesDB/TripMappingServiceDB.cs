using BusTrack.BusTrack.DB.Classes;
using MongoDB.Bson.Serialization;

namespace BusTrack.BusTrack.DB.ServicesDB
{
    public class TripMappingServiceDB
    {
        public static void RegisterTripMapping()
        {
            BsonClassMap.RegisterClassMap<TripDB>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}
