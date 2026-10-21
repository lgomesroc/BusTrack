using MongoDB.Bson;

namespace BusTrack.BusTrack.DB.InterfacesDB.IModelsDB
{
    public interface IUserModelDB
    {
        ObjectId Id { get; set; }
        string? FullName { get; set; }
        string? CPF { get; set; }
        string? Email { get; set; }
        string? Password { get; set; }
    }
}
