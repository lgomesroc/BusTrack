using System.Net;
using System.Text;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;
using Newtonsoft.Json;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class PassengerControllerAPIIntegrationTest
        : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PassengerControllerAPIIntegrationTest(
            CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("/api/Passenger");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsSuccessStatusCode()
        {
            var createdPassenger = await CreatePassengerAsync();

            var response = await _client.GetAsync(
                $"/api/Passenger/{createdPassenger.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            await _client.DeleteAsync($"/api/Passenger/{createdPassenger.Id}");
        }

        [Fact]
        public async Task Post_ReturnsCreatedStatusCode()
        {
            var createdPassenger = await CreatePassengerAsync();

            Assert.False(string.IsNullOrWhiteSpace(createdPassenger.Id));

            await _client.DeleteAsync($"/api/Passenger/{createdPassenger.Id}");
        }

        [Fact]
        public async Task Put_ReturnsSuccessStatusCode()
        {
            var createdPassenger = await CreatePassengerAsync();
            var updatedPassenger = new PassengerDTOAPI
            {
                Name = "Updated Test Passenger",
                Cpf = createdPassenger.Cpf,
                Email = createdPassenger.Email,
                Phone = createdPassenger.Phone
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(updatedPassenger),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PutAsync(
                $"/api/Passenger/{createdPassenger.Id}",
                content);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            await _client.DeleteAsync($"/api/Passenger/{createdPassenger.Id}");
        }

        [Fact]
        public async Task Delete_ReturnsNoContentStatusCode()
        {
            var createdPassenger = await CreatePassengerAsync();

            var response = await _client.DeleteAsync(
                $"/api/Passenger/{createdPassenger.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        private async Task<PassengerDTOAPI> CreatePassengerAsync()
        {
            var uniqueId = Guid.NewGuid().ToString("N");
            var passenger = new PassengerDTOAPI
            {
                Name = $"Test Passenger {uniqueId}",
                Cpf = uniqueId[..11],
                Email = $"passenger-{uniqueId}@example.com",
                Phone = "21999999999"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(passenger),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/Passenger", content);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var createdPassenger =
                JsonConvert.DeserializeObject<PassengerDTOAPI>(responseBody);

            Assert.NotNull(createdPassenger);
            Assert.False(string.IsNullOrWhiteSpace(createdPassenger.Id));

            return createdPassenger;
        }
    }
}
