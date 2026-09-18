using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.Tests.MappingsIntegrationTests;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BusTrack.Tests.IntegrationTests.ServicesAPIIntegrationTests
{
    public class RouteServiceAPIIntegrationTest
    {
        private readonly Mock<IRouteRepositoryDB> _routeRepository;
        private readonly IMapper _mapper;
        private readonly RouteServiceAPI _routeServiceAPI;

        public RouteServiceAPIIntegrationTest()
        {
            _routeRepository =
                new Mock<IRouteRepositoryDB>();

            var config =
                new MapperConfiguration(
                    cfg =>
                        cfg.AddProfile<AutoMapperProfile>(),
                    NullLoggerFactory.Instance);

            _mapper =
                config.CreateMapper();

            _routeServiceAPI =
                new RouteServiceAPI(
                    _routeRepository.Object,
                    _mapper);
        }

        [Fact]
        public async Task GetAllRoutes_ReturnsAllRoutes()
        {
            var routes =
                new List<RouteDB>
                {
                    new RouteDB(),
                    new RouteDB()
                };

            _routeRepository
                .Setup(x => x.GetAllRoutesAsync())
                .ReturnsAsync(routes);

            var result =
                await _routeServiceAPI.GetAllRoutes();

            Assert.Equal(
                2,
                result.Count());
        }

        [Fact]
        public async Task GetRouteById_ReturnsRoute()
        {
            var id = "1";

            var route =
                new RouteDB
                {
                    Id = id,
                    Name = "Route 1",
                    Origin = "Origin",
                    Destination = "Destination"
                };

            _routeRepository
                .Setup(x =>
                    x.GetRouteByIdAsync(id))
                .ReturnsAsync(route);

            var result =
                await _routeServiceAPI.GetRouteById(id);

            Assert.NotNull(result);
            Assert.Equal(
                id,
                result.Id);
        }

        [Fact]
        public async Task GetById_ReturnsRoute()
        {
            var id = "1";

            var route =
                new RouteDB
                {
                    Id = id,
                    Name = "Route 1",
                    Origin = "Origin",
                    Destination = "Destination"
                };

            _routeRepository
                .Setup(x =>
                    x.GetRouteByIdAsync(id))
                .ReturnsAsync(route);

            var result =
                await _routeServiceAPI.GetRouteById(id);

            Assert.NotNull(result);
            Assert.Equal(
                id,
                result.Id);
        }

        [Fact]
        public async Task CreateRoute_ReturnsAddedRoute()
        {
            var routeDTO =
                new RouteDTOAPI
                {
                    Name = "Route Name",
                    Origin = "Origin",
                    Destination = "Destination",
                    Distance = 100
                };

            var routeDB =
                _mapper.Map<RouteDB>(
                    routeDTO);

            _routeRepository
                .Setup(x =>
                    x.CreateRoute(
                        It.IsAny<RouteDB>()))
                .ReturnsAsync(routeDB);

            var result =
                await _routeServiceAPI.CreateRoute(
                    routeDTO);

            Assert.NotNull(result);
            Assert.Equal(
                routeDTO.Name,
                result.Name);
            Assert.Equal(
                routeDTO.Origin,
                result.Origin);
            Assert.Equal(
                routeDTO.Destination,
                result.Destination);
        }

        [Fact]
        public async Task UpdateRoute_ReturnsUpdatedRoute()
        {
            var routeId = "routeId";

            var routeDTO =
                new RouteDTOAPI
                {
                    Name = "Updated Name",
                    Origin = "Updated Origin",
                    Destination = "Updated Destination",
                    Distance = 150
                };

            var existingRoute =
                new RouteDB
                {
                    Id = routeId,
                    Name = "Old Name",
                    Origin = "Old Origin",
                    Destination = "Old Destination"
                };

            _routeRepository
                .Setup(x =>
                    x.GetRouteByIdAsync(routeId))
                .ReturnsAsync(existingRoute);

            _routeRepository
                .Setup(x =>
                    x.UpdateRouteAsync(
                        routeId,
                        It.IsAny<RouteDB>()))
                .ReturnsAsync(
                    (string id, RouteDB route) =>
                        route);

            var result =
                await _routeServiceAPI.UpdateRoute(
                    routeId,
                    routeDTO);

            Assert.NotNull(result);
            Assert.Equal(
                routeDTO.Name,
                result.Name);
            Assert.Equal(
                routeDTO.Origin,
                result.Origin);
            Assert.Equal(
                routeDTO.Destination,
                result.Destination);
        }

        [Fact]
        public async Task DeleteRoute_ReturnsTrueWhenDeleted()
        {
            var routeId = "routeId";

            _routeRepository
                .Setup(x =>
                    x.GetRouteByIdAsync(routeId))
                .ReturnsAsync(
                    new RouteDB
                    {
                        Id = routeId
                    });

            _routeRepository
                .Setup(x =>
                    x.DeleteRouteAsync(routeId))
                .Returns(Task.CompletedTask);

            var result =
                await _routeServiceAPI.DeleteRoute(
                    routeId);

            Assert.True(result);
        }
    }
}
