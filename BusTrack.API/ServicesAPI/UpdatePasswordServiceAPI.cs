using System.Text;
using BusTrack.BusTrack.DB.ClassesDB;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class UpdatePasswordServiceAPI : IUpdatePasswordServiceAPI
    {
        private readonly IMongoCollection<UserPasswordHistoryDB> _userCollection;

        public UpdatePasswordServiceAPI(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("BusTrack");
            _userCollection = database.GetCollection<UserPasswordHistoryDB>("Users");
        }

        public async void UpdatePassword(int userId, string newPassword)
        {
            // 1. Buscar o usuário pelo userId
            var filter = Builders<UserPasswordHistoryDB>.Filter.Eq(u => u.Id, userId);
            var user = (await _userCollection.FindAsync(filter)).ToList().FirstOrDefault();

            if (user == null)
            {
                throw new Exception("Usuário não encontrado");
            }

            // 2. Validar se a nova senha não está na fila
            if (user.PasswordHistories.Any(ph => ph.PasswordHash == BCrypt.Net.BCrypt.HashPassword(newPassword)))
            {
                throw new Exception("A senha já foi utilizada recentemente. Por favor, escolha uma senha diferente.");
            }

            // 3. Hash da nova senha
            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // 4. Atualizar a data de criação da última senha
            if (user.PasswordHistories.Count > 0)
            {
                user.PasswordHistories.Last().CreationDate = DateTime.Now;
            }

            // 5. Adicionar a nova senha à fila
            user.PasswordHistories.Insert(0, new PasswordHistoryDB
            {
                PasswordHash = newPasswordHash,
                CreationDate = DateTime.Now
            });

            // 6. Limitar a fila de senhas a 5 elementos
            if (user.PasswordHistories.Count > 5)
            {
                user.PasswordHistories.RemoveAt(5);
            }

            // 7. Atualizar o usuário no banco de dados (MongoDB)
            var update = Builders<UserPasswordHistoryDB>.Update.Set(u => u.PasswordHistories, user.PasswordHistories);
            await _userCollection.UpdateOneAsync(filter, update);

            // 8. Verificar se a data de criação da última senha está próxima de completar 45 dias
            if (user.PasswordHistories.Count > 0 &&
                (DateTime.Now - user.PasswordHistories.Last().CreationDate).Days >= 40)
            {
                // Enviar aviso para o usuário sobre a necessidade de trocar a senha
                Console.WriteLine("Sua senha atual está próxima de completar 45 dias de uso. Por motivos de segurança, recomendamos que você troque sua senha o mais rápido possível.");
            }
        }
    }
}