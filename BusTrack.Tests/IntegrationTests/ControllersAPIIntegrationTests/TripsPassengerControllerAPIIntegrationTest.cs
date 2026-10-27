using System.Net;
using System.Text;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class TripsPassengerControllerAPIIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TripsPassengerControllerAPIIntegrationTest(CustomWebApplicationFactory<Program> factory) => _client = factory.CreateClient();

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("/api/TripsPassenger");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var tripPassenger = await CreateTripPassengerAsync();
            var response = await _client.GetAsync($"/api/TripsPassenger/{tripPassenger.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsCreatedStatusCode()
        {
            var tripId = await CreateTripAsync();
            var response = await PostAsync("/api/TripsPassenger", new { TripId = tripId, PassengerId = ObjectId.GenerateNewId().ToString() });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            var tripPassenger = await CreateTripPassengerAsync();
            var response = await PutAsync($"/api/TripsPassenger/{tripPassenger.Id}", new { TripId = tripPassenger.TripId, PassengerId = ObjectId.GenerateNewId().ToString() });
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNoContentStatusCode()
        {
            var tripPassenger = await CreateTripPassengerAsync();
            var response = await _client.DeleteAsync($"/api/TripsPassenger/{tripPassenger.Id}");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        private async Task<(string Id, string TripId, string PassengerId)> CreateTripPassengerAsync()
        {
            var tripId = await CreateTripAsync();
            var passengerId = ObjectId.GenerateNewId().ToString();
            var response = await PostAsync("/api/TripsPassenger", new { TripId = tripId, PassengerId = passengerId });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await ReadTripPassengerAsync(response);
            return (created.Id!, tripId, passengerId);
        }

        private async Task<string> CreateTripAsync()
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

            var tripResponse = await PostAsync("/api/Trip", new
            {
                BusId = bus.Id,
                DriverId = driver.Id,
                RouteId = route.Id,
                DepartureTime = DateTime.UtcNow.AddMinutes(10),
                ArrivalTime = (DateTime?)null,
                Duration = 0,
                LimitPassengers = 0,
                Passengers = new List<string>()
            });
            Assert.Equal(HttpStatusCode.Created, tripResponse.StatusCode);
            var trip = await ReadEntityAsync(tripResponse);
            return trip.Id!;
        }

        private async Task<HttpResponseMessage> PostAsync(string path, object value)
        {
            var content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
            return await _client.PostAsync(path, content);
        }

        private async Task<HttpResponseMessage> PutAsync(string path, object value)
        {
            var content = new StringContent(JsonConvert.SerializeObject(value), Encoding.UTF8, "application/json");
            return await _client.PutAsync(path, content);
        }

        private async Task<EntityResponse> ReadEntityAsync(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            var entity = JsonConvert.DeserializeObject<EntityResponse>(body);
            Assert.NotNull(entity);
            Assert.False(string.IsNullOrWhiteSpace(entity!.Id));
            return entity;
        }

        private async Task<TripPassengerResponse> ReadTripPassengerAsync(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            var created = JsonConvert.DeserializeObject<TripPassengerResponse>(body);
            Assert.NotNull(created);
            Assert.False(string.IsNullOrWhiteSpace(created!.Id));
            return created;
        }

        private sealed class EntityResponse { public string? Id { get; set; } }
        private sealed class TripPassengerResponse { public string? Id { get; set; } public string? TripId { get; set; } public string? PassengerId { get; set; } }
    }
}
