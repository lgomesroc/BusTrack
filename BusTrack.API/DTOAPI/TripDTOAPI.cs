namespace BusTrack.BusTrack.API.DTOAPI
{
    public class TripDTOAPI
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? BusId { get; set; }
        public string? DriverId { get; set; }
        public string? RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
        public int PlannedDurationMinutes { get; set; } = 60;
        public DateTime? StartedAt { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public int Duration { get; set; }
        public int LimitPassengers { get; set; }
        public string Status { get; set; } = "Agendada";
        public List<string>? Passengers { get; set; }
    }
}
