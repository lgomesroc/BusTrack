using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusTrack.BusTrack.DB.Classes
{
    public class BusDB
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? Number { get; set; }

        public string? Plate { get; set; }

        public string? Line { get; set; }

        public List<int> Routes { get; set; }
    }
}
