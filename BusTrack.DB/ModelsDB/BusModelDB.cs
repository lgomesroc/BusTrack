using BusTrack.BusTrack.DB.InterfacesDB.IModelsDB;

namespace BusTrack.BusTrack.DB.ModelsDB
{
    public class BusModelDB : IBusModelDB
    {
        public int Id { get; set; }
        public string? Number { get; set; }
        public string? Plate { get; set; }
        public string? Line { get; set; }
        public List<string>? Routes { get; set; }
    }
}
