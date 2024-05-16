using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ModelsAPI;
using BusTrack.BusTrack.DB.ConnectionsDB;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class AccountServiceAPI : IAccountServiceAPI
    {
        private readonly IMongoCollection<AccountModelsAPI> _accounts;
        private readonly ConnectionDB _connectionDB;

        public AccountServiceAPI(string connectionString, string databaseName)
        {
            _connectionDB = new ConnectionDB();
            var database = _connectionDB.Connect(connectionString, databaseName);
            _accounts = database.GetCollection<AccountModelsAPI>("Accounts");
        }

        public IActionResult Get()
        {
            var accounts = _accounts.Find(account => true).ToList();
            return new OkObjectResult(accounts);
        }

        public AccountModelsAPI GetByEmail(string email) =>
            _accounts.Find<AccountModelsAPI>(account => account.Email == email).FirstOrDefault();

        public void CreateAccount(AccountModelsAPI account)
        {
            _accounts.InsertOne(account);
        }

        public IActionResult GetAllAccounts() =>
            Get();

        public AccountModelsAPI GetAccountById(string id)
        {
            var account = _accounts.Find<AccountModelsAPI>(account => account.Id == id).FirstOrDefault();
            return account;
        }

        public bool UpdateAccount(string id, AccountModelsAPI accountIn)
        {
            var result = _accounts.ReplaceOne(account => account.Id == id, accountIn);
            return result.ModifiedCount > 0;
        }

        public bool DeleteAccount(string id)
        {
            var result = _accounts.DeleteOne(account => account.Id == id);
            return result.DeletedCount > 0;
        }
    }
}