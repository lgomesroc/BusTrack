using System.Text;
using System.Net;
using Newtonsoft.Json;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class TripControllerAPIIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TripControllerAPIIntegrationTest(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {

            var response = await _client.GetAsync("/api/Trip");

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var id = 1;

            var response = await _client.GetAsync($"/api/Trip/{id}");

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsSuccessStatusCode()
        {
            var trip = new { Origin = "Origin", Destination = "Destination", DepartureTime = DateTime.Now };
            var content = new StringContent(JsonConvert.SerializeObject(trip), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Trip", content);

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            var id = 1;
            var trip = new { Origin = "Updated Origin", Destination = "Updated Destination", DepartureTime = DateTime.Now };
            var content = new StringContent(JsonConvert.SerializeObject(trip), Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/Trip/{id}", content);

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsSuccessStatusCode()
        {
            var id = 1;

            var response = await _client.DeleteAsync($"/api/Trip/{id}");

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
