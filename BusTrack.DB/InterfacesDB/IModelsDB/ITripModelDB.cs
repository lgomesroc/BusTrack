namespace BusTrack.BusTrack.DB.InterfacesDB.IModelsDB
{
    public interface ITripModelDB
    {
        int Id { get; set; }
        int BusId { get; set; }
        DateTime DepartureTime { get; set; }
        DateTime ArrivalTime { get; set; }
        int Duration { get; set; }
        int LimitPassengers { get; set; }
        List<int> Passengers { get; set; }
        int DriverId { get; set; }
    }
}
