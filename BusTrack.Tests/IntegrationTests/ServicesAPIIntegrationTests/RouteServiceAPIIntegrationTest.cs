using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Moq;
using MongoDB.Driver;
using BusTrack.Tests.MappingsIntegrationTests;
using System.Net;

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
            // Arrange
            var routes = new List<RouteDB> { new RouteDB(), new RouteDB() };
            _routeRepository.Setup(x => x.GetAllRoutesAsync()).ReturnsAsync(routes);

            // Act
            var result = await _routeServiceAPI.GetAllRoutes();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetRouteById_ReturnsRoute()
        {
            // Arrange
            var id = 1;

            // Act
            var response = await _client.GetAsync($"/api/Driver/{id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            // Arrange
            var id = "1";

            // Act
            var response = await _client.GetAsync($"/api/Route/{id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task CreateRoute_ReturnsAddedRoute()
        {
            // Arrange
            var routeDTO = new RouteDTOAPI { Name = "Route Name", Description = "Route Description" };
            var routeDB = _mapper.Map<RouteDB>(routeDTO);
            _routeRepository.Setup(x => x.CreateRoute(It.IsAny<RouteDB>())).ReturnsAsync(routeDB);

            // Act
            var result = await _routeServiceAPI.CreateRoute(routeDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(routeDTO.Name, result.Name);
            Assert.Equal(routeDTO.Description, result.Description);
        }

        [Fact]
        public async Task UpdateRoute_ReturnsUpdatedRoute()
        {
            // Arrange
            var routeId = "routeId";
            var routeDTO = new RouteDTOAPI { Name = "Updated Name", Description = "Updated Description" };
            var existingRoute = new RouteDB { Id = routeId };

            _routeRepository.Setup(x => x.UpdateRouteAsync(routeId, It.IsAny<RouteDB>())).ReturnsAsync(existingRoute);

            // Act
            var result = await _routeServiceAPI.UpdateRoute(routeId, routeDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(routeDTO.Name, result.Name);
            Assert.Equal(routeDTO.Description, result.Description);
        }

        [Fact]
        public async Task DeleteRoute_ReturnsTrueWhenDeleted()
        {
            // Arrange
            var routeId = "routeId";
            _routeRepository.Setup(x => x.DeleteRoute(routeId)).ReturnsAsync(true);

            // Act
            var result = await _routeServiceAPI.DeleteRoute(routeId);

            // Assert
            Assert.True(result);
        }
    }
}
