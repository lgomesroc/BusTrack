namespace BusTrack.BusTrack.DB.InterfacesDB.IModelsDB
{
    public interface IRouteModelDB
    {
        int Id { get; set; }
        string Name { get; set; }
        string Origin { get; set; }
        string Destination { get; set; }
    }
}
