using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusTrack.BusTrack.DB.Classes
{
    public class TripDB
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? BusId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? DriverId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? RouteId { get; set; }

        public DateTime DepartureTime { get; set; }

        public int PlannedDurationMinutes { get; set; } = 60;

        // Compatibilidade de leitura com documentos antigos que usam PlannedDuration.
        [BsonElement("PlannedDuration")]
        [BsonIgnoreIfNull]
        public int? LegacyPlannedDuration
        {
            get => null;
            set
            {
                if (value.HasValue)
                {
                    PlannedDurationMinutes = value.Value;
                }
            }
        }

        public DateTime? StartedAt { get; set; }

        public DateTime? ArrivalTime { get; set; }

        public int Duration { get; set; }

        public int LimitPassengers { get; set; }

        public string Status { get; set; } = "Agendada";

        public List<string>? Passengers { get; set; }
    }
}
