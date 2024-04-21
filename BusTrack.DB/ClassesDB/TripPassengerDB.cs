using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusTrack.BusTrack.DB.Classes
{
    public class TripPassengerDB
    {
        public string? TripId { get; set; }
        public string? PassengerId { get; set; }
    }
}
