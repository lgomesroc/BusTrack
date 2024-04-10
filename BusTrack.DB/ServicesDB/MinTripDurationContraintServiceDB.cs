using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.ServicrDB
{
    public class MinTripDurationContraintServiceDB
    {
        public static void ApplyMinTripDurationConstraint(IMongoDatabase database)
        {
            var tripsCollection = database.GetCollection<BsonDocument>("Trips");

            var filter = Builders<BsonDocument>.Filter.Empty; // Filtro vazio para atualizar todos os documentos
            var update = Builders<BsonDocument>.Update.Inc("arrivalTime", TimeSpan.FromHours(1)); // Adiciona uma hora à arrivalTime

            tripsCollection.UpdateMany(filter, update);
        }
    }

}
