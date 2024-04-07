using BusTrack.BusTrack.DB.InterfacesDB.IModelsDB;

namespace BusTrack.BusTrack.DB.ModelsDB
{
    public class PassengerModelDB : IPassengerModelDB
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
