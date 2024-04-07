namespace BusTrack.BusTrack.DB.InterfacesDB.IModelsDB
{
    public interface IBusModelDB
    {
        int Id { get; set; }
        string Number { get; set; }
        string Plate { get; set; }
        string Line { get; set; }
        List<string> Routes { get; set; }
    }
}
