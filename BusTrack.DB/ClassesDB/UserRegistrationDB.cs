namespace BusTrack.BusTrack.DB.ClassesDB
{
    public class UserRegistrationDB
    { 
        public string Email { get; } // O e-mail do usuário, que será o login
        public List<PasswordRecordDB> Passwords { get; } // Estrutura de dados para armazenar as senhas
        public Queue<DateTime> PasswordCreationDates { get; } // Estrutura de dados para armazenar as datas de criação das senhas

        public UserRegistrationDB(string email)
        {
            Email = email;
            Passwords = new List<PasswordRecordDB>();
            PasswordCreationDates = new Queue<DateTime>();
        }

        // Método para adicionar uma nova senha
        public void AddPassword(string passwordHash, DateTime creationDate)
        {
            // Verifica se a nova senha já está na fila
            bool passwordExists = false;
            foreach (var passwordRecord in Passwords)
            {
                if (passwordRecord.Hash == passwordHash)
                {
                    passwordExists = true;
                    break;
                }
            }

            // Se a senha não existe na fila
            if (!passwordExists)
            {
                // Verifica se a fila está cheia
                if (Passwords.Count == 5)
                {
                    // Remove a senha mais antiga e sua data de criação
                    Passwords.RemoveAt(0);
                    PasswordCreationDates.Dequeue();
                }

                // Adiciona a nova senha e sua data de criação
                PasswordRecordDB newRecord = new PasswordRecordDB(passwordHash, creationDate);
                Passwords.Add(newRecord);
                PasswordCreationDates.Enqueue(creationDate);
            }
        }

        // Método para verificar se a senha atual precisa ser atualizada
        public bool NeedsPasswordUpdate(string newPasswordHash, DateTime newPasswordCreationDate)
        {
            // Verifica se a nova senha já existe na fila
            foreach (var passwordRecord in Passwords)
            {
                if (passwordRecord.Hash == newPasswordHash)
                {
                    return true; // Senha igual já existe na fila
                }
            }

            // Verifica se a senha atual tem mais de 45 dias
            if (PasswordCreationDates.Count > 0)
            {
                DateTime oldestPasswordCreationDate = PasswordCreationDates.Peek();
                TimeSpan timeSinceCreation = newPasswordCreationDate - oldestPasswordCreationDate;
                if (timeSinceCreation.TotalDays > 45)
                {
                    return true; // Senha atual tem mais de 45 dias
                }
            }

            return false; // Senha não precisa ser atualizada
        }
    }
}
