using BusTrack.BusTrack.DB.InterfacesDB.IModelsDB;

namespace BusTrack.BusTrack.DB.ModelsDB
{
    public class TripModelDB : ITripModelDB
    {
        public int Id { get; set; }
        public int BusId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int Duration { get; set; }
        public int LimitPassengers { get; set; }
        public List<int> Passengers { get; set; }
        public int DriverId { get; set; }
    }
}
