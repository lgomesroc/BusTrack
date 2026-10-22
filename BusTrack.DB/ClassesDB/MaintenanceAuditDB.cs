
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusTrack.BusTrack.DB.Classes
{
    public class MaintenanceAuditDB
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string MaintenanceId { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;

        public string PerformedBy { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public List<MaintenanceAuditChangeDB> Changes { get; set; } = [];

        public BsonDocument? Snapshot { get; set; }
    }

    public class MaintenanceAuditChangeDB
    {
        public string Field { get; set; } = string.Empty;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }
    }
}
