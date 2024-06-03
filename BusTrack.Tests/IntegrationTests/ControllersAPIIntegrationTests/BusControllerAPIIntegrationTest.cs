using System.Text;
using System.Net;
using Newtonsoft.Json;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;

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

            var response = await _client.GetAsync("/api/Bus");

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var id = 1;

            var response = await _client.GetAsync($"/api/Bus/{id}");

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsSuccessStatusCode()
        {
            var bus = new { Name = "Test Bus", Capacity = 50 };
            var content = new StringContent(JsonConvert.SerializeObject(bus), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Bus", content);

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            var id = 1;
            var bus = new { Name = "Updated Test Bus", Capacity = 60 };
            var content = new StringContent(JsonConvert.SerializeObject(bus), Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/Bus/{id}", content);

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsSuccessStatusCode()
        {
            var id = 1;

            var response = await _client.DeleteAsync($"/api/Bus/{id}");

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
