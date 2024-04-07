using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusTrack.BusTrack.DB.Classes
{
    public class TripPassengerDB
    {
        public int TripId { get; set; }
        public int PassengerId { get; set; }
    }
}
