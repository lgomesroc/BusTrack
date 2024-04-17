using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ModelsAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI
{
    public interface IRouteServiceAPI
    {
        Task<IEnumerable<RouteDTOAPI>> GetAllRoutes();

        Task<RouteDTOAPI> GetRouteById(int id);

        Task<RouteDTOAPI> CreateRoute(RouteDTOAPI route);

        Task<RouteDTOAPI> UpdateRouteAsync(string id, RouteModelAPI route);
        Task<bool> DeleteRoute(int id);

        List<RouteDB> GetRoutes();
    }
}
