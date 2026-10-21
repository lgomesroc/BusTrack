using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using BusTrack.BusTrack.DB.InterfacesDB.IModelsDB;

namespace BusTrack.BusTrack.DB.ModelsDB
{
    public class AccountModelDB : IAccountModelDB
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
