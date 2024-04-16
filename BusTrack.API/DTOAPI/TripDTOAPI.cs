namespace BusTrack.BusTrack.API.DTOAPI
{
    public class TripDTOAPI
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public int DriverId { get; set; }
        public int RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
    }
}
