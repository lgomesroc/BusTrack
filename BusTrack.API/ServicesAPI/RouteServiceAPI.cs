using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using BusTrack.BusTrack.API.ModelsAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class RouteServiceAPI : IRouteServiceAPI
    {
        private readonly IRouteRepositoryDB _routeRepository;
        private readonly IMapper _mapper;

        public RouteServiceAPI(IRouteRepositoryDB routeRepository, IMapper mapper)
        {
            _routeRepository = routeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RouteDTOAPI>> GetAllRoutes()
        {
            var routes = await _routeRepository.GetAllRoutesAsync();
            return _mapper.Map<IEnumerable<RouteDTOAPI>>(routes);
        }


        public async Task<RouteDTOAPI> GetRouteById(string id)
        {
            var route = await _routeRepository.GetRouteByIdAsync(id.ToString());
            return _mapper.Map<RouteDTOAPI>(route);
        }

        public async Task<RouteDTOAPI> CreateRoute(RouteDTOAPI route)
        {
            var routeModel = _mapper.Map<RouteModelAPI>(route);
            var routeDB = _mapper.Map<RouteDB>(routeModel);
            var createdRouteDB = await _routeRepository.CreateRoute(routeDB);
            return _mapper.Map<RouteDTOAPI>(createdRouteDB);
        }


        public async Task<RouteDTOAPI> UpdateRoute(string id, RouteDTOAPI route)
        {
            var routeModel = _mapper.Map<RouteModelAPI>(route);
            var routeDB = _mapper.Map<RouteDB>(routeModel);
            var updatedRoute = await _routeRepository.UpdateRouteAsync(id, routeDB);
            return _mapper.Map<RouteDTOAPI>(updatedRoute);
        }

        public async Task<RouteDTOAPI> UpdateRouteAsync(string id, RouteModelAPI routeModel)
        {
            var routeDB = _mapper.Map<RouteDB>(routeModel);
            var updatedRouteDB = await _routeRepository.UpdateRouteAsync(id, routeDB);
            return _mapper.Map<RouteDTOAPI>(updatedRouteDB);
        }

        public async Task<bool> DeleteRoute(int id)
        {
            return await _routeRepository.DeleteRoute(id);
        }

        public async Task<List<RouteDB>> GetRoutes()
        {
            var route = (await _routeRepository.GetAllRoutesAsync()).ToList();
            return route;
        }
    }
}