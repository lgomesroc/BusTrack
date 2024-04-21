using System.Text;
using System.Net;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;
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
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/Trip");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            // Arrange
            var id = 1;

            // Act
            var response = await _client.GetAsync($"/api/Trip/{id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsSuccessStatusCode()
        {
            // Arrange
            var trip = new { Origin = "Origin", Destination = "Destination", DepartureTime = DateTime.Now };
            var content = new StringContent(JsonConvert.SerializeObject(trip), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Trip", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            // Arrange
            var id = 1;
            var trip = new { Origin = "Updated Origin", Destination = "Updated Destination", DepartureTime = DateTime.Now };
            var content = new StringContent(JsonConvert.SerializeObject(trip), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/Trip/{id}", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsSuccessStatusCode()
        {
            // Arrange
            var id = 1;

            // Act
            var response = await _client.DeleteAsync($"/api/Trip/{id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
