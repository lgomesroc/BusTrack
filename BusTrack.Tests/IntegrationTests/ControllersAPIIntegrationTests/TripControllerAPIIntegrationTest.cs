using System.Net;
using System.Text;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;
using Newtonsoft.Json;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class TripControllerAPIIntegrationTest
        : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TripControllerAPIIntegrationTest(
            CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {
            var response =
                await _client.GetAsync("/api/Trip");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var id = await CreateTripAsync();

            var response =
                await _client.GetAsync(
                    $"/api/Trip/{id}");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsCreatedStatusCode()
        {
            var dependencies =
                await CreateTripDependenciesAsync();

            var trip = new
            {
                BusId = dependencies.BusId,
                DriverId = dependencies.DriverId,
                RouteId = dependencies.RouteId,
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(1),
                Duration = 60,
                LimitPassengers = 40,
                Passengers = new List<string>()
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(trip),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PostAsync(
                    "/api/Trip",
                    content);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            var tripData =
                await CreateTripWithDependenciesAsync();

            var trip = new
            {
                BusId = tripData.BusId,
                DriverId = tripData.DriverId,
                RouteId = tripData.RouteId,
                DepartureTime = DateTime.UtcNow.AddHours(1),
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                Duration = 60,
                LimitPassengers = 50,
                Passengers = new List<string>()
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(trip),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PutAsync(
                    $"/api/Trip/{tripData.TripId}",
                    content);

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNoContentStatusCode()
        {
            var tripData =
                await CreateTripWithDependenciesAsync();

            var response =
                await _client.DeleteAsync(
                    $"/api/Trip/{tripData.TripId}");

            Assert.Equal(
                HttpStatusCode.NoContent,
                response.StatusCode);
        }

        private async Task<string> CreateTripAsync()
        {
            var tripData =
                await CreateTripWithDependenciesAsync();

            return tripData.TripId;
        }

        private async Task<
            (string BusId, string DriverId, string RouteId)>
            CreateTripDependenciesAsync()
        {
            var bus = new
            {
                Number = $"BUS-{Guid.NewGuid():N}",
                LicensePlate = "ABC1234",
                Model = "Integration Test Model",
                Capacity = 50
            };

            var busContent =
                new StringContent(
                    JsonConvert.SerializeObject(bus),
                    Encoding.UTF8,
                    "application/json");

            var busResponse =
                await _client.PostAsync(
                    "/api/Bus",
                    busContent);

            Assert.Equal(
                HttpStatusCode.Created,
                busResponse.StatusCode);

            var busBody =
                await busResponse.Content
                    .ReadAsStringAsync();

            var createdBus =
                JsonConvert.DeserializeObject<EntityResponse>(
                    busBody);

            Assert.NotNull(createdBus);
            Assert.False(
                string.IsNullOrWhiteSpace(
                    createdBus!.Id));

            var driver = new
            {
                Name = $"Integration Driver {Guid.NewGuid()}",
                LicenseNumber = "ABC123"
            };

            var driverContent =
                new StringContent(
                    JsonConvert.SerializeObject(driver),
                    Encoding.UTF8,
                    "application/json");

            var driverResponse =
                await _client.PostAsync(
                    "/api/Driver",
                    driverContent);

            Assert.Equal(
                HttpStatusCode.Created,
                driverResponse.StatusCode);

            var driverBody =
                await driverResponse.Content
                    .ReadAsStringAsync();

            var createdDriver =
                JsonConvert.DeserializeObject<EntityResponse>(
                    driverBody);

            Assert.NotNull(createdDriver);
            Assert.False(
                string.IsNullOrWhiteSpace(
                    createdDriver!.Id));

            var route = new
            {
                Name = $"Integration Route {Guid.NewGuid()}",
                Description = "Integration test route",
                Origin = "Origin",
                Destination = "Destination",
                Distance = 100
            };

            var routeContent =
                new StringContent(
                    JsonConvert.SerializeObject(route),
                    Encoding.UTF8,
                    "application/json");

            var routeResponse =
                await _client.PostAsync(
                    "/api/Route",
                    routeContent);

            Assert.Equal(
                HttpStatusCode.Created,
                routeResponse.StatusCode);

            var routeBody =
                await routeResponse.Content
                    .ReadAsStringAsync();

            var createdRoute =
                JsonConvert.DeserializeObject<EntityResponse>(
                    routeBody);

            Assert.NotNull(createdRoute);
            Assert.False(
                string.IsNullOrWhiteSpace(
                    createdRoute!.Id));

            return (
                createdBus.Id!,
                createdDriver.Id!,
                createdRoute.Id!);
        }

        private async Task<
            (string TripId, string BusId, string DriverId, string RouteId)>
            CreateTripWithDependenciesAsync()
        {
            var dependencies =
                await CreateTripDependenciesAsync();

            var trip = new
            {
                BusId = dependencies.BusId,
                DriverId = dependencies.DriverId,
                RouteId = dependencies.RouteId,
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(1),
                Duration = 60,
                LimitPassengers = 40,
                Passengers = new List<string>()
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(trip),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PostAsync(
                    "/api/Trip",
                    content);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);

            var responseBody =
                await response.Content
                    .ReadAsStringAsync();

            var createdTrip =
                JsonConvert.DeserializeObject<EntityResponse>(
                    responseBody);

            Assert.NotNull(createdTrip);
            Assert.False(
                string.IsNullOrWhiteSpace(
                    createdTrip!.Id));

            return (
                createdTrip.Id!,
                dependencies.BusId,
                dependencies.DriverId,
                dependencies.RouteId);
        }

        private sealed class EntityResponse
        {
            public string? Id { get; set; }
        }
    }
}
