namespace BusTrack.BusTrack.DB.InterfacesDB.IModelsDB
{
    public interface IInspectorModelDB
    {
        string? Id { get; set; }
        string? FullName { get; set; }
        string? CPF { get; set; }
        string? Email { get; set; }
        string? Password { get; set; }
    }
}
