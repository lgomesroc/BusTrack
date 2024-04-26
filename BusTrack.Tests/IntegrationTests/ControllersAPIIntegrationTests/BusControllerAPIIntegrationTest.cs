using System.Text;
using System.Net;
using Newtonsoft.Json;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class BusControllerAPIIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BusControllerAPIIntegrationTest(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/Bus");

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
            var response = await _client.GetAsync($"/api/Bus/{id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsSuccessStatusCode()
        {
            // Arrange
            var bus = new { Name = "Test Bus", Capacity = 50 };
            var content = new StringContent(JsonConvert.SerializeObject(bus), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Bus", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            // Arrange
            var id = 1;
            var bus = new { Name = "Updated Test Bus", Capacity = 60 };
            var content = new StringContent(JsonConvert.SerializeObject(bus), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/Bus/{id}", content);

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
            var response = await _client.DeleteAsync($"/api/Bus/{id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
