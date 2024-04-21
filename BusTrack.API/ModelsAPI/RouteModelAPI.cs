namespace BusTrack.BusTrack.API.ModelsAPI
{
    public class RouteModelAPI
    {
        public int Id { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public double Distance { get; set; }
    }
}
