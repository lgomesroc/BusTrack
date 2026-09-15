using BusTrack.BusTrack.DB.ClassesDB;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class UserRepositoryDB : IUserRepositoryDB
    {
        private readonly IMongoCollection<UserDB> _users;

        public UserRepositoryDB(IMongoDatabase database)
        {
            _users = database.GetCollection<UserDB>("Accounts");
        }

        public List<UserDB> Get() =>
            _users.Find(user => true).ToList();

        public UserDB? GetByEmail(string email) =>
            _users.Find(user => user.Email == email).FirstOrDefault();

        public UserDB Create(UserDB user)
        {
            _users.InsertOne(user);
            return user;
        }

        public UserDB? Read(string id) =>
            _users.Find(user => user.Id == id).FirstOrDefault();

        public bool Update(string id, UserDB user)
        {
            var result = _users.ReplaceOne(
                user => user.Id == id,
                user
            );

            return result.ModifiedCount > 0;
        }

        public bool Delete(string id)
        {
            var result = _users.DeleteOne(
                user => user.Id == id
            );

            return result.DeletedCount > 0;
        }
    }
}
