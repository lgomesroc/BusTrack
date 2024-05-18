using BenchmarkDotNet.Attributes;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using Microsoft.Extensions.DependencyInjection;

namespace BusTrack.BusTrack.Tests.PerfomanceTests
{
    public class RouteServiceAPIPerformanceTests
    {
        private readonly IRouteServiceAPI _routeService;

        public RouteServiceAPIPerformanceTests()
        {
            var services = new ServiceCollection();
            services.AddScoped<IRouteServiceAPI, RouteServiceAPI>();
            // Adicione outras dependências necessárias
            var serviceProvider = services.BuildServiceProvider();
            _routeService = serviceProvider.GetRequiredService<IRouteServiceAPI>();
        }

        [Benchmark]
        public async Task AddRouteTest()
        {
            var routeDto = new RouteDTOAPI
            {
                Origin = "Performance City A",
                Destination = "Performance City B",
                Distance = 100
            };

            await _routeService.AddRouteAsync(routeDto);
        }

        [Benchmark]
        public async Task GetRouteTest()
        {
            var routeId = 1; // Assuming the ID of the route to retrieve
            await _routeService.GetRouteAsync(routeId);
        }

        [Benchmark]
        public async Task UpdateRouteTest()
        {
            var routeId = 1; // Assuming the ID of the route to update
            var updatedRouteDto = new RouteDTOAPI
            {
                Id = routeId.ToString(), // Assuming Id is an integer
                Origin = "Updated Performance City A",
                Destination = "Updated Performance City B",
                Distance = 150
            };
        }

        [Benchmark]
        public async Task DeleteRouteTest()
        {
            var routeId = 1; // Assuming the ID of the route to delete
            await _routeService.DeleteRouteAsync(routeId);
        }
    }
}
