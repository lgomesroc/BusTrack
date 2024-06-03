namespace BusTrack.BusTrack.DB.ClassesDB
{
    public class PasswordHistoryDB
    {
    public int Id { get; set; }
    public string? PasswordHash { get; set; }
    public DateTime CreationDate { get; set; }
    }
}