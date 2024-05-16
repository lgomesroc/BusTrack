using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BusTrack.BusTrack.DB.ClassesDB
{
    public class UserDB
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? FullName { get; set; }

        public string? CPF { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Login { get; set; } // Adicionando o campo Login
    }
}
