using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BusTrack.BusTrack.DB.ModelsDB
{
    public class AccountModelDB
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Email")]
        public string? Email { get; set; }

        [BsonElement("Password")]
        public string? Password { get; set; }

        public AccountModelDB()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }
    }
}
