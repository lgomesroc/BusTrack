

namespace BusTrack.BusTrack.DB.ClassesDB
{
    public class UserPasswordHistoryDB
    {
       public int Id { get; set; }
       public string? Email { get; set; }
       public List<PasswordHistoryDB> PasswordHistories { get; set; }
    }
}