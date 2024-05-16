using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusTrack.BusTrack.API.ModelsAPI
{
    public class AccountModelsAPI
    { 
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? FullName { get; set; }

        public string? CPF { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }
    }
}
