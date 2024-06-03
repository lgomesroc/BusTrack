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
            var routeId = 1; 
            await _routeService.GetRouteAsync(routeId);
        }

        [Benchmark]
        public async Task UpdateRouteTest()
        {
            var routeId = 1; 
            var updatedRouteDto = new RouteDTOAPI
            {
                Id = routeId.ToString(),
                Origin = "Updated Performance City A",
                Destination = "Updated Performance City B",
                Distance = 150
            };
        }

        [Benchmark]
        public async Task DeleteRouteTest()
        {
            var routeId = 1; 
            await _routeService.DeleteRouteAsync(routeId);
        }
    }
}
