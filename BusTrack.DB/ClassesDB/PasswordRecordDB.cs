namespace BusTrack.BusTrack.DB.ClassesDB
{
    public class PasswordRecordDB
    {
        public string Hash { get; set; } 
        public DateTime CreationDate { get; set; } 

        public PasswordRecordDB(string hash, DateTime creationDate)
        {
            Hash = hash;
            CreationDate = creationDate;
        }
    }
}
