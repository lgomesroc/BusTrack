namespace BusTrack.BusTrack.DB.InterfacesDB.IModelsDB
{
    public interface IPassengerModelDB
    {
        int Id { get; set; }
        string Name { get; set; }
        string Cpf { get; set; }
        string Email { get; set; }
        string Phone { get; set; }
    }
}
