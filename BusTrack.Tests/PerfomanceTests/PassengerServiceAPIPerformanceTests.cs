using BenchmarkDotNet.Attributes;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using Microsoft.Extensions.DependencyInjection;

namespace BusTrack.BusTrack.Tests.PerfomanceTests
{
    [MemoryDiagnoser]
    public class PassengerServiceAPIPerformanceTests
    {
        private readonly IPassengerServiceAPI _passengerService;

        public PassengerServiceAPIPerformanceTests()
        {
            var services = new ServiceCollection();
            services.AddScoped<IPassengerServiceAPI, PassengerServiceAPI>();
            // Adicione outras dependências necessárias
            var serviceProvider = services.BuildServiceProvider();
            _passengerService = serviceProvider.GetRequiredService<IPassengerServiceAPI>();
        }

        [Benchmark]
        public async Task AddPassengerTest()
        {
            var passengerDto = new PassengerDTOAPI
            {
                Name = "Performance Test",
                Email = "performance.test@example.com",
                Age = 30
            };

            await _passengerService.AddPassengerAsync(passengerDto);
        }

        [Benchmark]
        public async Task GetPassengerTest()
        {
            // Assumindo que existe um passageiro com ID 1
            await _passengerService.GetPassengerByIdAsync(1);
        }

        [Benchmark]
        public async Task UpdatePassengerTest()
        {
            var passengerDto = new PassengerDTOAPI
            {
                Id = "1", // Assumindo que existe um passageiro com ID 1
                Name = "Updated Performance Test",
                Email = "updated.performance.test@example.com",
                Age = 31
            };

            await _passengerService.UpdatePassengerAsync(passengerDto);
        }

        [Benchmark]
        public async Task DeletePassengerTest()
        {
            // Assumindo que existe um passageiro com ID 1
            await _passengerService.DeletePassengerAsync(1);
        }

        [Benchmark]
        public async Task ListPassengersTest()
        {
            await _passengerService.GetAllPassengersAsync();
        }
    }
}
