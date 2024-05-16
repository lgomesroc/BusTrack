using MongoDB.Bson;

namespace BusTrack.BusTrack.API.ModelsAPI
{
    public class UserModelAPI
    {
        public ObjectId Id { get; set; }
        public string? FullName { get; set; }
        public string? CPF { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
