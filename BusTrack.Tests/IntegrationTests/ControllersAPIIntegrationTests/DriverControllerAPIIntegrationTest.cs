using System.Text;
using System.Net;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class DriverControllerAPIIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DriverControllerAPIIntegrationTest(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/Driver");

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
            var response = await _client.GetAsync($"/api/Driver/{id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsSuccessStatusCode()
        {
            // Arrange
            var driver = new { Name = "Test Driver", LicenseNumber = "ABC123" };
            var content = new StringContent(JsonConvert.SerializeObject(driver), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Driver", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            // Arrange
            var id = 1;
            var driver = new { Name = "Updated Test Driver", LicenseNumber = "XYZ789" };
            var content = new StringContent(JsonConvert.SerializeObject(driver), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/Driver/{id}", content);

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
            var response = await _client.DeleteAsync($"/api/Driver/{id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
