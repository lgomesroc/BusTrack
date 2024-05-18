using BusTrack.BusTrack.API.DTOAPI;
using MongoDB.Driver;

namespace BusTrack.BusTrack.API.ModelsAPI
{
    public class RouteModelAPI
    {
        public int Id { get; set; }
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public double Distance { get; set; }
        private readonly IMongoCollection<RouteDTOAPI> _routesCollection;

        public RouteModelAPI(IMongoDatabase database)
        {
            _routesCollection = database.GetCollection<RouteDTOAPI>("routes");
        }

        public async Task UpdateRouteAsync(int routeId, RouteDTOAPI updatedRouteDto)
        {
            var filter = Builders<RouteDTOAPI>.Filter.Eq("Id", routeId);
            var update = Builders<RouteDTOAPI>.Update
                .Set("Origin", updatedRouteDto.Origin)
                .Set("Destination", updatedRouteDto.Destination)
                .Set("Distance", updatedRouteDto.Distance);

            await _routesCollection.UpdateOneAsync(filter, update);
        }
    }
}
