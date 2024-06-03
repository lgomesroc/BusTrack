using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Moq;
using MongoDB.Driver;
using BusTrack.Tests.MappingsIntegrationTests;

namespace BusTrack.Tests.IntegrationTests.ServicesAPIIntegrationTests
{
    public class RouteServiceAPIIntegrationTest
    {
        private Mock<IRouteRepositoryDB> _routeRepository;
        private IMapper _mapper;
        private RouteServiceAPI _routeServiceAPI;

        public RouteServiceAPIIntegrationTest()
        {
            _routeRepository = new Mock<IRouteRepositoryDB>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            _mapper = config.CreateMapper();
            _routeServiceAPI = new RouteServiceAPI(_routeRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetAllRoutes_ReturnsAllRoutes()
        {
            var routes = new List<RouteDB> { new RouteDB(), new RouteDB() };
            _routeRepository.Setup(x => x.GetAllRoutesAsync()).ReturnsAsync(routes);

            var result = await _routeServiceAPI.GetAllRoutes();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetRouteById_ReturnsRoute()
        {
            var id = 1;

            var result = await _routeServiceAPI.GetRouteById(id.ToString());

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var id = "1";

            var result = await _routeServiceAPI.GetRouteById(id);

            Assert.NotNull(result);
        }


        [Fact]
        public async Task CreateRoute_ReturnsAddedRoute()
        {
            var routeDTO = new RouteDTOAPI { Name = "Route Name", Description = "Route Description" };
            var routeDB = _mapper.Map<RouteDB>(routeDTO);
            _routeRepository.Setup(x => x.CreateRoute(It.IsAny<RouteDB>())).ReturnsAsync(routeDB);

            var result = await _routeServiceAPI.CreateRoute(routeDTO);

            Assert.NotNull(result);
            Assert.Equal(routeDTO.Name, result.Name);
            Assert.Equal(routeDTO.Description, result.Description);
        }

        [Fact]
        public async Task UpdateRoute_ReturnsUpdatedRoute()
        {
            var routeId = "routeId";
            var routeDTO = new RouteDTOAPI { Name = "Updated Name", Description = "Updated Description" };
            var existingRoute = new RouteDB { Id = routeId };

            _routeRepository.Setup(x => x.UpdateRouteAsync(routeId, It.IsAny<RouteDB>())).ReturnsAsync(existingRoute);

            var result = await _routeServiceAPI.UpdateRoute(routeId, routeDTO);

            Assert.NotNull(result);
            Assert.Equal(routeDTO.Name, result.Name);
            Assert.Equal(routeDTO.Description, result.Description);
        }

        [Fact]
        public async Task DeleteRoute_ReturnsTrueWhenDeleted()
        {
            var routeId = "routeId";
            _routeRepository.Setup(x => x.DeleteRoute(Int32.Parse(routeId))).ReturnsAsync(true);

            var result = await _routeServiceAPI.DeleteRoute((Int32.Parse(routeId)));

            Assert.True(result);
        }
    }
}
