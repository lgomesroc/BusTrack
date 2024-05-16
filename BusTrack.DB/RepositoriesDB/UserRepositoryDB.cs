using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.ClassesDB;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class UserRepositoryDB
    {
        private readonly IMongoCollection<UserDB> _users;
        public UserRepositoryDB(IMongoDatabase database)
        {
            _users = database.GetCollection<UserDB>("Users");
        }

        public List<UserDB> Get() =>
            _users.Find(user => true).ToList();

        public UserDB GetByLogin(string login) =>
            _users.Find<UserDB>(user => user.Login == login).FirstOrDefault();

        public UserDB Create(UserDB user)
        {
            _users.InsertOne(user);
            return user;
        }
    }
}
