using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BusTrack.BusTrack.DB.ClassesDB
{
        public class EmailConfirmationDB
        {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? UserId { get; set; }

        public string? Email { get; set; }


        public string? ConfirmationCode { get; set; }

        public DateTime? SentAt { get; set; }
    }
}