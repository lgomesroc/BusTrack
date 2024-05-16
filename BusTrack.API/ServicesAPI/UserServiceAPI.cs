using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.ModelsDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class UserServiceAPI : IUserServiceAPI
    {
        private readonly IMongoCollection<UserModelDB> _userCollection;

        public UserServiceAPI(IMongoDatabase database)
        {
            _userCollection = database.GetCollection<UserModelDB>("Users");
        }

        public async Task<UserModelDB> Authenticate(string email, string password)
        {
            // Implemente a lógica de autenticação aqui
            return null;
        }

        public async Task CreateUser(UserModelDB user)
        {
            await _userCollection.InsertOneAsync(user);
        }

        public async Task<List<UserModelDB>> GetAll()
        {
            return await _userCollection.Find(_ => true).ToListAsync();
        }

        public async Task<UserModelDB> GetById(string id)
        {
            var objectId = new ObjectId(id);
            return await _userCollection.Find<UserModelDB>(user => user.Id == objectId).FirstOrDefaultAsync();
        }

        public async Task Update(string id, UserModelDB user)
        {
            var objectId = new ObjectId(id);
            await _userCollection.ReplaceOneAsync(u => u.Id == objectId, user);
        }

        public async Task Delete(string id)
        {
            var objectId = new ObjectId(id);
            await _userCollection.DeleteOneAsync(u => u.Id == objectId);
        }
    }
}
