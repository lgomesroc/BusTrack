using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Newtonsoft.Json;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests;

public class MaintenanceControllerAdditionalIntegrationTests : IDisposable
{
    private const string MaintenanceId = "507f1f77bcf86cd799439011";
    private const string TestKey = "BusTrack-test-only-signing-key-not-for-production-2026";
    private const string TestIssuer = "BusTrack.Tests";
    private const string TestAudience = "BusTrack.Tests.Client";
    private const string DeleteReason = "Registro de teste";

    private readonly MaintenanceTestFactory _factory = new();
    private readonly HttpClient _anonymousClient;

    public MaintenanceControllerAdditionalIntegrationTests()
    {
        _anonymousClient = _factory.CreateClient();
    }

    [Fact]
    public async Task Get_WhenTokenIsMissing_ReturnsUnauthorized()
    {
        using var response = await _anonymousClient.GetAsync("/api/Maintenance");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAudit_WhenUserIsNotAdmin_ReturnsForbidden()
    {
        using var client = CreateClient("USER");
        using var response = await client.GetAsync($"/api/Maintenance/{MaintenanceId}/audit");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        _factory.ServiceMock.Verify(
            service => service.GetAuditAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Put_WhenServiceRejectsExistingMaintenance_ReturnsBadRequest()
    {
        _factory.ServiceMock
            .Setup(service => service.UpdateAsync(
                MaintenanceId,
                It.IsAny<MaintenanceDTOAPI>()))
            .ReturnsAsync((MaintenanceDTOAPI?)null);
        _factory.ServiceMock
            .Setup(service => service.GetByIdAsync(MaintenanceId))
            .ReturnsAsync(new MaintenanceDTOAPI
            {
                Id = MaintenanceId,
                Status = "Agendada"
            });

        using var client = CreateClient("ADMIN");
        using var content = CreateJsonContent(CreateMaintenance());
        using var response = await client.PutAsync(
            $"/api/Maintenance/{MaintenanceId}",
            content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_WhenMaintenanceDoesNotExist_ReturnsNotFound()
    {
        _factory.ServiceMock
            .Setup(service => service.UpdateAsync(
                MaintenanceId,
                It.IsAny<MaintenanceDTOAPI>()))
            .ReturnsAsync((MaintenanceDTOAPI?)null);
        _factory.ServiceMock
            .Setup(service => service.GetByIdAsync(MaintenanceId))
            .ReturnsAsync((MaintenanceDTOAPI?)null);

        using var client = CreateClient("ADMIN");
        using var content = CreateJsonContent(CreateMaintenance());
        using var response = await client.PutAsync(
            $"/api/Maintenance/{MaintenanceId}",
            content);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenServiceRejectsExistingMaintenance_ReturnsBadRequest()
    {
        _factory.ServiceMock
            .Setup(service => service.DeleteAsync(MaintenanceId, DeleteReason))
            .ReturnsAsync(false);
        _factory.ServiceMock
            .Setup(service => service.GetByIdAsync(MaintenanceId))
            .ReturnsAsync(new MaintenanceDTOAPI
            {
                Id = MaintenanceId,
                Status = "Em andamento"
            });

        using var client = CreateClient("ADMIN");
        using var response = await client.DeleteAsync(
            $"/api/Maintenance/{MaintenanceId}?reason={Uri.EscapeDataString(DeleteReason)}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private HttpClient CreateClient(string role)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CreateToken(role));
        return client;
    }

    private static string CreateToken(string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestKey));
        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, "507f1f77bcf86cd799439099"),
                new Claim(ClaimTypes.Name, "Maintenance Integration Test"),
                new Claim(ClaimTypes.Role, role)
            ],
            notBefore: DateTime.UtcNow.AddMinutes(-1),
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static MaintenanceDTOAPI CreateMaintenance() => new()
    {
        Id = MaintenanceId,
        BusId = "507f1f77bcf86cd799439012",
        BusNumber = "001",
        ServiceType = "Revisão",
        Description = "Teste de integração",
        ServiceDate = DateTime.UtcNow.AddDays(2),
        Cost = 250m,
        Status = "Agendada"
    };

    private static StringContent CreateJsonContent(MaintenanceDTOAPI maintenance) =>
        new(JsonConvert.SerializeObject(maintenance), Encoding.UTF8, "application/json");

    public void Dispose()
    {
        _anonymousClient.Dispose();
        _factory.Dispose();
    }

    private sealed class MaintenanceTestFactory : WebApplicationFactory<Program>
    {
        public Mock<IMaintenanceServiceAPI> ServiceMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JwtSettings:SecretKey"] = TestKey,
                    ["JwtSettings:Issuer"] = TestIssuer,
                    ["JwtSettings:Audience"] = TestAudience,
                    ["JwtSettings:ExpirationInMinutes"] = "30",
                    ["BootstrapAdmin:Secret"] = null
                }));

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IMaintenanceServiceAPI>();
                services.AddSingleton<IMaintenanceServiceAPI>(ServiceMock.Object);
            });
        }
    }
}
