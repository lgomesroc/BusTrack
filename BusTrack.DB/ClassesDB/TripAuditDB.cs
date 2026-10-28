using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusTrack.BusTrack.DB.Classes
{
    public class TripAuditDB
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("TripId")]
        public string TripId { get; set; } = string.Empty;

        [BsonElement("Action")]
        public string Action { get; set; } = string.Empty;

        [BsonElement("OccurredAtUtc")]
        public DateTime OccurredAtUtc { get; set; }

        [BsonElement("PerformedBy")]
        public string PerformedBy { get; set; } = string.Empty;

        [BsonElement("Reason")]
        public string? Reason { get; set; }

        [BsonElement("Snapshot")]
        public BsonDocument Snapshot { get; set; } = new();

        [BsonElement("Changes")]
        public List<TripAuditChangeDB> Changes { get; set; } = [];
    }

    public class TripAuditChangeDB
    {
        [BsonElement("Field")]
        public string Field { get; set; } = string.Empty;

        [BsonElement("OldValue")]
        public string? OldValue { get; set; }

        [BsonElement("NewValue")]
        public string? NewValue { get; set; }
    }
}