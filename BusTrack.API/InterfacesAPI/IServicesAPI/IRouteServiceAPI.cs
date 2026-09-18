using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface IRouteServiceAPI
    {
        Task<IEnumerable<RouteDTOAPI>> GetAllRoutes();

        Task<RouteDTOAPI?> GetRouteById(
            string? id);

        Task<RouteDTOAPI> CreateRoute(
            RouteDTOAPI route);

        Task<RouteDTOAPI?> UpdateRoute(
            string? id,
            RouteDTOAPI route);

        Task<bool> DeleteRoute(
            string? id);

        Task<List<RouteDB>> GetRoutes();
    }
}
