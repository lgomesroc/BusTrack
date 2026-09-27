using System.Net;
using System.Text;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;
using Newtonsoft.Json;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class RouteControllerAPIIntegrationTest
        : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public RouteControllerAPIIntegrationTest(
            CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {
            var response =
                await _client.GetAsync("/api/Route");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var id = await CreateRouteAsync();

            var response =
                await _client.GetAsync(
                    $"/api/Route/{id}");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsCreatedStatusCode()
        {
            var route = new
            {
                Name = $"Test Route {Guid.NewGuid()}",
                Description = "Integration test route",
                Origin = "Origin",
                Destination = "Destination",
                Distance = 100
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(route),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PostAsync(
                    "/api/Route",
                    content);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            var id = await CreateRouteAsync();

            var route = new
            {
                Name = $"Updated Route {Guid.NewGuid()}",
                Description = "Updated integration test route",
                Origin = "Updated Origin",
                Destination = "Updated Destination",
                Distance = 120
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(route),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PutAsync(
                    $"/api/Route/{id}",
                    content);

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNoContentStatusCode()
        {
            var id = await CreateRouteAsync();

            var response =
                await _client.DeleteAsync(
                    $"/api/Route/{id}");

            Assert.Equal(
                HttpStatusCode.NoContent,
                response.StatusCode);
        }

        private async Task<string> CreateRouteAsync()
        {
            var route = new
            {
                Name = $"Integration Route {Guid.NewGuid()}",
                Description = "Integration test route",
                Origin = "Origin",
                Destination = "Destination",
                Distance = 100
            };

            var content =
                new StringContent(
                    JsonConvert.SerializeObject(route),
                    Encoding.UTF8,
                    "application/json");

            var response =
                await _client.PostAsync(
                    "/api/Route",
                    content);

            Assert.Equal(
                HttpStatusCode.Created,
                response.StatusCode);

            var responseBody =
                await response.Content.ReadAsStringAsync();

            var createdRoute =
                JsonConvert.DeserializeObject<RouteResponse>(
                    responseBody);

            Assert.NotNull(createdRoute);
            Assert.False(
                string.IsNullOrWhiteSpace(
                    createdRoute!.Id));

            return createdRoute.Id!;
        }

        private sealed class RouteResponse
        {
            public string? Id { get; set; }
        }
    }
}
