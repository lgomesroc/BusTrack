using System;
using System.Linq;
using System.Threading.Tasks;

using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.ClassesDB;

using MongoDB.Driver;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class UpdatePasswordServiceAPI :
        IUpdatePasswordServiceAPI
    {
        private readonly IMongoCollection<UserPasswordHistoryDB>
            _userCollection;

        public UpdatePasswordServiceAPI(
            IMongoDatabase database)
        {
            _userCollection =
                database.GetCollection<UserPasswordHistoryDB>(
                    "Users");
        }

        public async void UpdatePassword(
            int userId,
            string newPassword)
        {
            var filter =
                Builders<UserPasswordHistoryDB>
                    .Filter
                    .Eq(
                        u => u.Id,
                        userId);

            var user =
                (await _userCollection
                    .FindAsync(filter))
                    .ToList()
                    .FirstOrDefault();

            if (user == null)
            {
                throw new Exception(
                    "Usuário não encontrado");
            }

            var passwordAlreadyUsed =
                user.PasswordHistories
                    .Any(
                        ph =>
                            BCrypt.Net.BCrypt.Verify(
                                newPassword,
                                ph.PasswordHash));

            if (passwordAlreadyUsed)
            {
                throw new Exception(
                    "A senha já foi utilizada recentemente. " +
                    "Por favor, escolha uma senha diferente.");
            }

            string newPasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    newPassword);

            if (user.PasswordHistories.Count > 0)
            {
                user.PasswordHistories
                    .Last()
                    .CreationDate =
                    DateTime.Now;
            }

            user.PasswordHistories.Insert(
                0,
                new PasswordHistoryDB
                {
                    PasswordHash =
                        newPasswordHash,

                    CreationDate =
                        DateTime.Now
                });

            if (user.PasswordHistories.Count > 5)
            {
                user.PasswordHistories.RemoveAt(5);
            }

            var update =
                Builders<UserPasswordHistoryDB>
                    .Update
                    .Set(
                        u =>
                            u.PasswordHistories,
                        user.PasswordHistories);

            await _userCollection
                .UpdateOneAsync(
                    filter,
                    update);

            if (
                user.PasswordHistories.Count > 0 &&
                (
                    DateTime.Now -
                    user.PasswordHistories
                        .Last()
                        .CreationDate
                ).Days >= 40)
            {
                Console.WriteLine(
                    "Sua senha atual está próxima de completar " +
                    "45 dias de uso. Por motivos de segurança, " +
                    "recomendamos que você troque sua senha o " +
                    "mais rápido possível.");
            }
        }
    }
}
