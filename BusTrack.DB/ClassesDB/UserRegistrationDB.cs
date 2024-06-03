namespace BusTrack.BusTrack.DB.ClassesDB
{
    public class UserRegistrationDB
    { 
        public string Email { get; } 
        public List<PasswordRecordDB> Passwords { get; } 
        public Queue<DateTime> PasswordCreationDates { get; } 

        public UserRegistrationDB(string email)
        {
            Email = email;
            Passwords = new List<PasswordRecordDB>();
            PasswordCreationDates = new Queue<DateTime>();
        }

        public void AddPassword(string passwordHash, DateTime creationDate)
        {
            bool passwordExists = false;
            foreach (var passwordRecord in Passwords)
            {
                if (passwordRecord.Hash == passwordHash)
                {
                    passwordExists = true;
                    break;
                }
            }

            if (!passwordExists)
            {
                if (Passwords.Count == 5)
                {
                    Passwords.RemoveAt(0);
                    PasswordCreationDates.Dequeue();
                }

                PasswordRecordDB newRecord = new PasswordRecordDB(passwordHash, creationDate);
                Passwords.Add(newRecord);
                PasswordCreationDates.Enqueue(creationDate);
            }
        }

        public bool NeedsPasswordUpdate(string newPasswordHash, DateTime newPasswordCreationDate)
        {
            foreach (var passwordRecord in Passwords)
            {
                if (passwordRecord.Hash == newPasswordHash)
                {
                    return true; 
                }
            }

            if (PasswordCreationDates.Count > 0)
            {
                DateTime oldestPasswordCreationDate = PasswordCreationDates.Peek();
                TimeSpan timeSinceCreation = newPasswordCreationDate - oldestPasswordCreationDate;
                if (timeSinceCreation.TotalDays > 45)
                {
                    return true; 
                }
            }

            return false; 
        }
    }
}
