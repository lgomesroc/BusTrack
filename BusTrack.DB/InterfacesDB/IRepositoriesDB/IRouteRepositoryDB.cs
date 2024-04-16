using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB
{
    public interface IRouteRepositoryDB
    {
        Task<IEnumerable<RouteDB>> GetAllRoutesAsync();
        Task<RouteDB> GetRouteByIdAsync(string id);
        Task AddRouteAsync(RouteDB route);
        Task<RouteDB> UpdateRouteAsync(string id, RouteDB route);
        Task DeleteRouteAsync(string id);
        Task<bool> DeleteRoute(int id);
        Task<RouteDB> CreateRoute(RouteDB route);

    }
}
