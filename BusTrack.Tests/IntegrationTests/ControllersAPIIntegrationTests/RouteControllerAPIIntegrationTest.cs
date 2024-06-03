using System.Text;
using System.Net;
using Newtonsoft.Json;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;

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

            var response = await _client.GetAsync("/api/Route");

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var id = 1;

            var response = await _client.GetAsync($"/api/Route/{id}");

            // Assert
            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsSuccessStatusCode()
        {
            
            var route = new { Name = "Test Route", Distance = 100 };
            var content = new StringContent(JsonConvert.SerializeObject(route), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Route", content);

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            var id = 1;
            var route = new { Name = "Updated Test Route", Distance = 120 };
            var content = new StringContent(JsonConvert.SerializeObject(route), Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/Route/{id}", content);

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsSuccessStatusCode()
        {
            var id = 1;

            var response = await _client.DeleteAsync($"/api/Route/{id}");

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
