using BusTrack.BusTrack.DB.InterfacesDB.IModelsDB;

namespace BusTrack.BusTrack.DB.ModelsDB
{
    public class TripPassengerModelDB : ITripPassengerModelDB
    {
        public int TripId { get; set; }
        public List<int> PassengerIds { get; set; }
    }
}
