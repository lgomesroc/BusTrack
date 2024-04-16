using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusTrack.BusTrack.DB.Classes
{
    public class DriverDB
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? Login { get; set; }

        public string? Name { get; set; }

        public string? Cpf { get; set; }

        public string? Email { get; set; }
    }
}
