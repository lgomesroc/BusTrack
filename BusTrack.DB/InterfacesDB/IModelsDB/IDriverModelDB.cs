namespace BusTrack.BusTrack.DB.InterfacesDB.IModelsDB
{
    public interface IDriverModelDB
    {
        int Id { get; set; }
        string Login { get; set; }
        string Name { get; set; }
        string Cpf { get; set; }
        string Email { get; set; }
    }
}
