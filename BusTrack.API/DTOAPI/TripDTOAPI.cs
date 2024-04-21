namespace BusTrack.BusTrack.API.DTOAPI
{
    public class TripDTOAPI
    {
        public string? Id { get; set; }
        public string? BusId { get; set; }
        public string? DriverId { get; set; }
        public string? RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
    }
}
