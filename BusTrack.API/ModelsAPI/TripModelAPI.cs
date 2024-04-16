namespace BusTrack.BusTrack.API.ModelsAPI
{
    public class TripModelAPI
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public int DriverId { get; set; }
        public int RouteId { get; set; }
        public DateTime DepartureTime { get; set; }
        public int PassengerCount { get; set; }
    }
}
