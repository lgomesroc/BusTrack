using System.Text;
using System.Net;
using Newtonsoft.Json;
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

            var response = await _client.GetAsync("/api/Passenger");

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var id = 1;

            var response = await _client.GetAsync($"/api/Passenger/{id}");

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsSuccessStatusCode()
        {
            var passenger = new { Name = "Test Passenger", Age = 30 };
            var content = new StringContent(JsonConvert.SerializeObject(passenger), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Passenger", content);

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            var id = 1;
            var passenger = new { Name = "Updated Test Passenger", Age = 35 };
            var content = new StringContent(JsonConvert.SerializeObject(passenger), Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/Passenger/{id}", content);

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsSuccessStatusCode()
        {
            var id = 1;

            var response = await _client.DeleteAsync($"/api/Passenger/{id}");

            response.EnsureSuccessStatusCode(); 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
