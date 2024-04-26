using System.Text;
using System.Net;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;
using BusTrack.BusTrack.Program;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class RouteControllerAPIIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public RouteControllerAPIIntegrationTest(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/Route");

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
            var response = await _client.GetAsync($"/api/Route/{id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsSuccessStatusCode()
        {
            // Arrange
            var route = new { Name = "Test Route", Distance = 100 };
            var content = new StringContent(JsonConvert.SerializeObject(route), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Route", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            // Arrange
            var id = 1;
            var route = new { Name = "Updated Test Route", Distance = 120 };
            var content = new StringContent(JsonConvert.SerializeObject(route), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/Route/{id}", content);

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
            var response = await _client.DeleteAsync($"/api/Route/{id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
