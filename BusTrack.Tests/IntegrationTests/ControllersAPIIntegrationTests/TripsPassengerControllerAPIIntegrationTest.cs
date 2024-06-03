using System.Text;
using System.Net;
using Newtonsoft.Json;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class TripsPassengerControllerAPIIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TripsPassengerControllerAPIIntegrationTest(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {

            var response = await _client.GetAsync("/api/TripsPassenger");

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var id = 1;

            var response = await _client.GetAsync($"/api/TripsPassenger/{id}");

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsSuccessStatusCode()
        {
            var tripPassenger = new { TripId = 1, PassengerId = 1 };
            var content = new StringContent(JsonConvert.SerializeObject(tripPassenger), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/TripsPassenger", content);

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            var id = 1;
            var tripPassenger = new { TripId = 2, PassengerId = 2 };
            var content = new StringContent(JsonConvert.SerializeObject(tripPassenger), Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/TripsPassenger/{id}", content);

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsSuccessStatusCode()
        {
            var id = 1;

            var response = await _client.DeleteAsync($"/api/TripsPassenger/{id}");

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
