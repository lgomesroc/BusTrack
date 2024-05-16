namespace BusTrack.BusTrack.DB.ClassesDB
{
    public class PasswordRecordDB
    {
        public string Hash { get; set; } // O hash da senha do usuário
        public DateTime CreationDate { get; set; } // A data de criação da senha

        public PasswordRecordDB(string hash, DateTime creationDate)
        {
            Hash = hash;
            CreationDate = creationDate;
        }
    }
}
