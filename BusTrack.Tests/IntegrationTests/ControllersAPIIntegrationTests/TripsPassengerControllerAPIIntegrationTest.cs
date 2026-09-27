using System.Net;
using System.Text;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class TripsPassengerControllerAPIIntegrationTest
        : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TripsPassengerControllerAPIIntegrationTest(
            CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {
            var response =
                await _client.GetAsync(
                    "/api/TripsPassenger");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var tripPassenger =
                await CreateTripPassengerAsync();

            var response =
                await _client.GetAsync(
                    $"/api/TripsPassenger/{tripPassenger.Id}");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsCreatedStatusCode()
        {
            var tripId =
                await CreateTripAsync();

            var passengerId =
                ObjectId.GenerateNewId()
                    .ToString();

            var tripPassenger = new
            {
                TripId = tripId,
                PassengerId = passengerId
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(
                        tripPassenger),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PostAsync(
                    "/api/TripsPassenger",
                    content);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            var tripPassenger =
                await CreateTripPassengerAsync();

            var updatedTripPassenger = new
            {
                TripId = tripPassenger.TripId,
                PassengerId =
                    ObjectId.GenerateNewId()
                        .ToString()
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(
                        updatedTripPassenger),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PutAsync(
                    $"/api/TripsPassenger/{tripPassenger.Id}",
                    content);

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNoContentStatusCode()
        {
            var tripPassenger =
                await CreateTripPassengerAsync();

            var response =
                await _client.DeleteAsync(
                    $"/api/TripsPassenger/{tripPassenger.Id}");

            Assert.Equal(
                HttpStatusCode.NoContent,
                response.StatusCode);
        }

        private async Task<
            (string Id, string TripId, string PassengerId)>
            CreateTripPassengerAsync()
        {
            var tripId =
                await CreateTripAsync();

            var passengerId =
                ObjectId.GenerateNewId()
                    .ToString();

            var tripPassenger = new
            {
                TripId = tripId,
                PassengerId = passengerId
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(
                        tripPassenger),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PostAsync(
                    "/api/TripsPassenger",
                    content);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);

            var responseBody =
                await response.Content
                    .ReadAsStringAsync();

            var created =
                JsonConvert.DeserializeObject<
                    TripPassengerResponse>(
                        responseBody);

            Assert.NotNull(created);
            Assert.False(
                string.IsNullOrWhiteSpace(
                    created!.Id));

            return (
                created.Id!,
                tripId,
                passengerId);
        }

        private async Task<string> CreateTripAsync()
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

            var trip = new
            {
                BusId = createdBus.Id,
                DriverId = createdDriver.Id,
                RouteId = createdRoute.Id,
                DepartureTime = DateTime.UtcNow,
                ArrivalTime = DateTime.UtcNow.AddHours(1),
                Duration = 60,
                LimitPassengers = 40,
                Passengers = new List<string>()
            };

            var tripContent =
                new StringContent(
                    JsonConvert.SerializeObject(trip),
                    Encoding.UTF8,
                    "application/json");

            var tripResponse =
                await _client.PostAsync(
                    "/api/Trip",
                    tripContent);

            Assert.Equal(
                HttpStatusCode.Created,
                tripResponse.StatusCode);

            var tripBody =
                await tripResponse.Content
                    .ReadAsStringAsync();

            var createdTrip =
                JsonConvert.DeserializeObject<EntityResponse>(
                    tripBody);

            Assert.NotNull(createdTrip);
            Assert.False(
                string.IsNullOrWhiteSpace(
                    createdTrip!.Id));

            return createdTrip.Id!;
        }

        private sealed class EntityResponse
        {
            public string? Id { get; set; }
        }

        private sealed class TripPassengerResponse
        {
            public string? Id { get; set; }

            public string? TripId { get; set; }

            public string? PassengerId { get; set; }
        }
    }
}
