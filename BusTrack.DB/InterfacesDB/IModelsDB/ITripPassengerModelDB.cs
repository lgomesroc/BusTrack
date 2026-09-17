namespace BusTrack.BusTrack.DB.InterfacesDB.IModelsDB
{
    public interface ITripPassengerModelDB
    {
        string? TripId { get; set; }

        List<string>? PassengerIds { get; set; }
    }
}
