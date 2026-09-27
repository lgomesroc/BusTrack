using System.Net;
using System.Text;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;
using Newtonsoft.Json;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class DriverControllerAPIIntegrationTest
        : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DriverControllerAPIIntegrationTest(
            CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {
            var response =
                await _client.GetAsync("/api/Driver");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var id = await CreateDriverAsync();

            var response =
                await _client.GetAsync(
                    $"/api/Driver/{id}");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsCreatedStatusCode()
        {
            var driver = new
            {
                Name = $"Test Driver {Guid.NewGuid()}",
                LicenseNumber = "ABC123"
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(driver),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PostAsync(
                    "/api/Driver",
                    content);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            var id = await CreateDriverAsync();

            var driver = new
            {
                Name = $"Updated Driver {Guid.NewGuid()}",
                LicenseNumber = "XYZ789"
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(driver),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PutAsync(
                    $"/api/Driver/{id}",
                    content);

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNoContentStatusCode()
        {
            var id = await CreateDriverAsync();

            var response =
                await _client.DeleteAsync(
                    $"/api/Driver/{id}");

            Assert.Equal(
                HttpStatusCode.NoContent,
                response.StatusCode);
        }

        private async Task<string> CreateDriverAsync()
        {
            var driver = new
            {
                Name = $"Integration Driver {Guid.NewGuid()}",
                LicenseNumber = "ABC123"
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(driver),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PostAsync(
                    "/api/Driver",
                    content);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);

            var responseBody =
                await response.Content.ReadAsStringAsync();

            var createdDriver =
                JsonConvert.DeserializeObject<DriverResponse>(
                    responseBody);

            Assert.NotNull(createdDriver);
            Assert.False(
                string.IsNullOrWhiteSpace(
                    createdDriver!.Id));

            return createdDriver.Id!;
        }

        private sealed class DriverResponse
        {
            public string? Id { get; set; }
        }
    }
}
