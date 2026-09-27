using System.Net;
using System.Text;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;
using Newtonsoft.Json;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class BusControllerAPIIntegrationTest
        : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BusControllerAPIIntegrationTest(
            CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {
            var response =
                await _client.GetAsync("/api/Bus");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var id = await CreateBusAsync();

            var response =
                await _client.GetAsync(
                    $"/api/Bus/{id}");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsCreatedStatusCode()
        {
            var bus = new
            {
                Number = $"BUS-{Guid.NewGuid():N}",
                LicensePlate = "ABC1234",
                Model = "Test Model",
                Capacity = 50
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(bus),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PostAsync(
                    "/api/Bus",
                    content);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            var id = await CreateBusAsync();

            var bus = new
            {
                Number = $"BUS-UPDATED-{Guid.NewGuid():N}",
                LicensePlate = "XYZ7890",
                Model = "Updated Test Model",
                Capacity = 60
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(bus),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PutAsync(
                    $"/api/Bus/{id}",
                    content);

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNoContentStatusCode()
        {
            var id = await CreateBusAsync();

            var response =
                await _client.DeleteAsync(
                    $"/api/Bus/{id}");

            Assert.Equal(
                HttpStatusCode.NoContent,
                response.StatusCode);
        }

        private async Task<string> CreateBusAsync()
        {
            var bus = new
            {
                Number = $"BUS-{Guid.NewGuid():N}",
                LicensePlate = "ABC1234",
                Model = "Integration Test Model",
                Capacity = 50
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(bus),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PostAsync(
                    "/api/Bus",
                    content);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);

            var responseBody =
                await response.Content.ReadAsStringAsync();

            var createdBus =
                JsonConvert.DeserializeObject<BusResponse>(
                    responseBody);

            Assert.NotNull(createdBus);
            Assert.False(
                string.IsNullOrWhiteSpace(
                    createdBus!.Id));

            return createdBus.Id!;
        }

        private sealed class BusResponse
        {
            public string? Id { get; set; }
        }
    }
}
