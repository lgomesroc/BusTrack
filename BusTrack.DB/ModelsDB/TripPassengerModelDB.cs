using BusTrack.BusTrack.DB.InterfacesDB.IModelsDB;

namespace BusTrack.BusTrack.DB.ModelsDB
{
    public class TripPassengerModelDB : ITripPassengerModelDB
    {
        public string? TripId { get; set; }

        public List<string>? PassengerIds { get; set; }
    }
}
