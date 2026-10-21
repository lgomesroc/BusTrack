using MongoDB.Bson;

namespace BusTrack.BusTrack.DB.InterfacesDB.IModelsDB
{
    public interface IAccountModelDB
    {
        string? Id { get; set; }
        string? Email { get; set; }
        string? Password { get; set; }
    }
}
