using BusTrack.BusTrack.DB.ClassesDB;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class InspectorRepositoryDB
    {
        private readonly IMongoCollection<InspectorDB> _inspectors;

        public InspectorRepositoryDB(IMongoDatabase database)
        {
            _inspectors = database.GetCollection<InspectorDB>("Inspectors");
        }

        public List<InspectorDB> Get() =>
            _inspectors.Find(inspector => true).ToList();

        public InspectorDB GetByCPF(string cpf) =>
            _inspectors.Find<InspectorDB>(inspector => inspector.CPF == cpf).FirstOrDefault();

        public InspectorDB Create(InspectorDB inspector)
        {
            _inspectors.InsertOne(inspector);
            return inspector;
        }
    }
}
