using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class RouteServiceAPI : IRouteServiceAPI
    {
        private readonly IRouteRepositoryDB _routeRepository;
        private readonly IMapper _mapper;

        public RouteServiceAPI(
            IRouteRepositoryDB routeRepository,
            IMapper mapper)
        {
            _routeRepository = routeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RouteDTOAPI>> GetAllRoutes()
        {
            var routes =
                await _routeRepository.GetAllRoutesAsync();

            return _mapper.Map<IEnumerable<RouteDTOAPI>>(
                routes);
        }

        public async Task<RouteDTOAPI?> GetRouteById(
            string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var route =
                await _routeRepository.GetRouteByIdAsync(id);

            if (route == null)
            {
                return null;
            }

            return _mapper.Map<RouteDTOAPI>(
                route);
        }

        public async Task<RouteDTOAPI> CreateRoute(
            RouteDTOAPI route)
        {
            var routeDB =
                _mapper.Map<RouteDB>(route);

            var createdRoute =
                await _routeRepository.CreateRoute(
                    routeDB);

            return _mapper.Map<RouteDTOAPI>(
                createdRoute);
        }

        public async Task<RouteDTOAPI?> UpdateRoute(
            string? id,
            RouteDTOAPI route)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var existingRoute =
                await _routeRepository.GetRouteByIdAsync(id);

            if (existingRoute == null)
            {
                return null;
            }

            _mapper.Map(
                route,
                existingRoute);

            existingRoute.Id = id;

            var updatedRoute =
                await _routeRepository.UpdateRouteAsync(
                    id,
                    existingRoute);

            if (updatedRoute == null)
            {
                return null;
            }

            return _mapper.Map<RouteDTOAPI>(
                updatedRoute);
        }

        public async Task<bool> DeleteRoute(
            string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            var existingRoute =
                await _routeRepository.GetRouteByIdAsync(id);

            if (existingRoute == null)
            {
                return false;
            }

            await _routeRepository.DeleteRouteAsync(id);

            return true;
        }

        public async Task<List<RouteDB>> GetRoutes()
        {
            var routes =
                await _routeRepository.GetAllRoutesAsync();

            return routes.ToList();
        }
    }
}
