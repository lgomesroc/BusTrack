using System.Net;
using System.Text;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;
using Newtonsoft.Json;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class TripControllerAPIIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TripControllerAPIIntegrationTest(CustomWebApplicationFactory<Program> factory) => _client = factory.CreateClient();

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("/api/Trip");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var id = await CreateTripAsync();
            var response = await _client.GetAsync($"/api/Trip/{id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsCreatedStatusCode()
        {
            var dependencies = await CreateTripDependenciesAsync();
            var trip = new
            {
                BusId = dependencies.BusId,
                DriverId = dependencies.DriverId,
                RouteId = dependencies.RouteId,
                DepartureTime = DateTime.UtcNow.AddMinutes(10),
                ArrivalTime = (DateTime?)null,
                Duration = 0,
                LimitPassengers = 0,
                Passengers = new List<string>()
            };
            var response = await PostTripAsync(trip);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            var tripData = await CreateTripWithDependenciesAsync();
            var trip = new
            {
                BusId = tripData.BusId,
                DriverId = tripData.DriverId,
                RouteId = tripData.RouteId,
                DepartureTime = tripData.DepartureTime,
                ArrivalTime = (DateTime?)null,
                Duration = 0,
                LimitPassengers = 0,
                Status = "Cancelada",
                Passengers = new List<string>()
            };
            var content = new StringContent(JsonConvert.SerializeObject(trip), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/api/Trip/{tripData.TripId}", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNoContentStatusCode()
        {
            var tripData = await CreateTripWithDependenciesAsync();
            var response = await _client.DeleteAsync($"/api/Trip/{tripData.TripId}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        private async Task<string> CreateTripAsync()
        {
            var tripData = await CreateTripWithDependenciesAsync();
            return tripData.TripId;
        }

        private async Task<(string BusId, string DriverId, string RouteId)> CreateTripDependenciesAsync()
        {
            var busResponse = await PostAsync("/api/Bus", new { Number = $"BUS-{Guid.NewGuid():N}", LicensePlate = "ABC1234", Model = "Integration Test Model", Capacity = 50 });
            Assert.Equal(HttpStatusCode.Created, busResponse.StatusCode);
            var bus = await ReadEntityAsync(busResponse);

            var driverResponse = await PostAsync("/api/Driver", new { Name = $"Integration Driver {Guid.NewGuid()}", LicenseNumber = "ABC123" });
            Assert.Equal(HttpStatusCode.Created, driverResponse.StatusCode);
            var driver = await ReadEntityAsync(driverResponse);

            var routeResponse = await PostAsync("/api/Route", new { Name = $"Integration Route {Guid.NewGuid()}", Description = "Integration test route", Origin = "Origin", Destination = "Destination", Distance = 100 });
            Assert.Equal(HttpStatusCode.Created, routeResponse.StatusCode);
            var route = await ReadEntityAsync(routeResponse);

            return (bus.Id!, driver.Id!, route.Id!);
        }

        private async Task<(string TripId, string BusId, string DriverId, string RouteId, DateTime DepartureTime)> CreateTripWithDependenciesAsync()
        {
            var dependencies = await CreateTripDependenciesAsync();
            var departure = DateTime.UtcNow.AddMinutes(10);
            var response = await PostTripAsync(new
            {
                BusId = dependencies.BusId,
                DriverId = dependencies.DriverId,
                RouteId = dependencies.RouteId,
                DepartureTime = departure,
                ArrivalTime = (DateTime?)null,
                Duration = 0,
                LimitPassengers = 0,
                Passengers = new List<string>()
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var trip = await ReadEntityAsync(response);
            return (trip.Id!, dependencies.BusId, dependencies.DriverId, dependencies.RouteId, departure);
        }

        private Task<HttpResponseMessage> PostTripAsync(object trip) => PostAsync("/api/Trip", trip);

        private async Task<HttpResponseMessage> PostAsync(string path, object value)
        {
            var content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
            return await _client.PostAsync(path, content);
        }

        private async Task<EntityResponse> ReadEntityAsync(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            var entity = JsonConvert.DeserializeObject<EntityResponse>(body);
            Assert.NotNull(entity);
            Assert.False(string.IsNullOrWhiteSpace(entity!.Id));
            return entity;
        }

        private sealed class EntityResponse { public string? Id { get; set; } }
    }
}
