using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusTrack.BusTrack.DB.Classes
{
    public class MaintenanceDB
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string BusId { get; set; } = string.Empty;

        public string BusNumber { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ServiceDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public decimal Cost { get; set; }
        public string Status { get; set; } = "Agendada";
    }
}