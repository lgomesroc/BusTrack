using System.Text;
using System.Net;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;


namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class PassengerControllerAPIIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PassengerControllerAPIIntegrationTest(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {
            // Arrange

            // Act
            var response = await _client.GetAsync("/api/Passenger");

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
            var response = await _client.GetAsync($"/api/Passenger/{id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsSuccessStatusCode()
        {
            // Arrange
            var passenger = new { Name = "Test Passenger", Age = 30 };
            var content = new StringContent(JsonConvert.SerializeObject(passenger), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Passenger", content);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            // Arrange
            var id = 1;
            var passenger = new { Name = "Updated Test Passenger", Age = 35 };
            var content = new StringContent(JsonConvert.SerializeObject(passenger), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/Passenger/{id}", content);

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
            var response = await _client.DeleteAsync($"/api/Passenger/{id}");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
