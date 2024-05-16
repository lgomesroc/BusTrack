using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusTrack.BusTrack.DB.ModelsDB
{
    public class UserModelDB
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string? FullName { get; set; }
        public string? CPF { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
