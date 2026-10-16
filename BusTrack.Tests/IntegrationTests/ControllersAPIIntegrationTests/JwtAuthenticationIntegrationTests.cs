using System.Net;
using System.Text;
using BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory;
using Newtonsoft.Json;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests
{
    public class JwtAuthenticationIntegrationTests
        : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public JwtAuthenticationIntegrationTests(
            CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
        {
            using var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = null;

            var response = await client.GetAsync("/api/Bus");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithInvalidToken_ReturnsUnauthorized()
        {
            using var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", "not.a.valid-token");

            var response = await client.GetAsync("/api/Bus");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UserRole_CannotUpdateBus_ReturnsForbidden()
        {
            using var client = _factory.CreateUserClient();
            using var content = new StringContent("{}", Encoding.UTF8, "application/json");

            var response = await client.PutAsync(
                "/api/Bus/000000000000000000000001",
                content);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UserRole_CannotDeleteBus_ReturnsForbidden()
        {
            using var client = _factory.CreateUserClient();

            var response = await client.DeleteAsync(
                "/api/Bus/000000000000000000000001");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task HealthEndpoint_WithoutToken_ReturnsSuccess()
        {
            using var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = null;

            var response = await client.GetAsync("/api/Health");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            using var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = null;
            var credentials = new { email = "missing-user@example.com", password = "invalid" };
            using var content = new StringContent(
                JsonConvert.SerializeObject(credentials),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                "/AuthenticationControllerAPI/login",
                content);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
