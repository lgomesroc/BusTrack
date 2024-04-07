namespace BusTrack.BusTrack.DB.InterfacesDB.IModelsDB
{
    public interface ITripPassengerModelDB
    {
        int TripId { get; set; }
        List<int> PassengerIds { get; set; }
    }
}
