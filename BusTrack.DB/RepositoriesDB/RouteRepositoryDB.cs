using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Driver;

namespace BusTrack.BusTrack.DB.RepositoriesDB
{
    public class RouteRepositoryDB : IRouteRepositoryDB
    {
        private readonly IMongoCollection<RouteDB> _routesCollection;

        public RouteRepositoryDB(
            IMongoDatabase database)
        {
            _routesCollection =
                database.GetCollection<RouteDB>("Routes");
        }

        public async Task<IEnumerable<RouteDB>> GetAllRoutesAsync()
        {
            return await _routesCollection
                .Find(_ => true)
                .ToListAsync();
        }

        public async Task<RouteDB?> GetRouteByIdAsync(
            string id)
        {
            var filter =
                Builders<RouteDB>
                    .Filter
                    .Eq(
                        route => route.Id,
                        id);

            return await _routesCollection
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task AddRouteAsync(
            RouteDB route)
        {
            await _routesCollection
                .InsertOneAsync(route);
        }

        public async Task<RouteDB> CreateRoute(
            RouteDB route)
        {
            await _routesCollection
                .InsertOneAsync(route);

            return route;
        }

        public async Task<RouteDB?> UpdateRouteAsync(
            string id,
            RouteDB route)
        {
            var filter =
                Builders<RouteDB>
                    .Filter
                    .Eq(
                        existingRoute => existingRoute.Id,
                        id);

            var result =
                await _routesCollection
                    .ReplaceOneAsync(
                        filter,
                        route);

            if (result.MatchedCount == 0)
            {
                return null;
            }

            return route;
        }

        public async Task DeleteRouteAsync(
            string id)
        {
            var filter =
                Builders<RouteDB>
                    .Filter
                    .Eq(
                        route => route.Id,
                        id);

            await _routesCollection
                .DeleteOneAsync(filter);
        }
    }
}
