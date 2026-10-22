
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

public class MaintenanceControllerAPIIntegrationTests : IDisposable
{
    private const string MaintenanceId = "507f1f77bcf86cd799439011";
    private const string TestKey =
        "BusTrack-test-only-signing-key-not-for-production-2026";
    private const string TestIssuer = "BusTrack.Tests";
    private const string TestAudience = "BusTrack.Tests.Client";
    private const string DeleteReason = "Registro criado por engano";

    private readonly MaintenanceTestFactory _factory;
    private readonly HttpClient _client;

    public MaintenanceControllerAPIIntegrationTests()
    {
        _factory = new MaintenanceTestFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Get_WhenAuthenticated_ReturnsOk()
    {
        _factory.ServiceMock
            .Setup(service => service.GetAllAsync())
            .ReturnsAsync(new List<MaintenanceDTOAPI>());

        using var client = CreateClient("USER");
        using var response = await client.GetAsync("/api/Maintenance");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenMaintenanceExists_ReturnsOk()
    {
        _factory.ServiceMock
            .Setup(service => service.GetByIdAsync(MaintenanceId))
            .ReturnsAsync(CreateMaintenance());

        using var client = CreateClient("USER");
        using var response = await client.GetAsync(
            $"/api/Maintenance/{MaintenanceId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenMaintenanceDoesNotExist_ReturnsNotFound()
    {
        _factory.ServiceMock
            .Setup(service => service.GetByIdAsync(MaintenanceId))
            .ReturnsAsync((MaintenanceDTOAPI?)null);

        using var client = CreateClient("USER");
        using var response = await client.GetAsync(
            $"/api/Maintenance/{MaintenanceId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_WhenAnonymous_ReturnsUnauthorized()
    {
        using var response = await _client.GetAsync("/api/Maintenance");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_WhenAdminAndDataIsValid_ReturnsCreated()
    {
        var maintenance = CreateMaintenance();

        _factory.ServiceMock
            .Setup(service => service.CreateAsync(
                It.IsAny<MaintenanceDTOAPI>()))
            .ReturnsAsync(maintenance);

        using var client = CreateClient("ADMIN");
        using var content = CreateJsonContent(maintenance);
        using var response = await client.PostAsync(
            "/api/Maintenance",
            content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_WhenUserRole_ReturnsForbidden()
    {
        using var client = CreateClient("USER");
        using var content = CreateJsonContent(CreateMaintenance());
        using var response = await client.PostAsync(
            "/api/Maintenance",
            content);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _factory.ServiceMock.Verify(
            service => service.CreateAsync(
                It.IsAny<MaintenanceDTOAPI>()),
            Times.Never);
    }

    [Fact]
    public async Task Post_WhenServiceRejectsData_ReturnsBadRequest()
    {
        _factory.ServiceMock
            .Setup(service => service.CreateAsync(
                It.IsAny<MaintenanceDTOAPI>()))
            .ReturnsAsync((MaintenanceDTOAPI?)null);

        using var client = CreateClient("ADMIN");
        using var content = CreateJsonContent(CreateMaintenance());
        using var response = await client.PostAsync(
            "/api/Maintenance",
            content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_WhenAdminAndUpdateSucceeds_ReturnsOk()
    {
        var maintenance = CreateMaintenance();

        _factory.ServiceMock
            .Setup(service => service.UpdateAsync(
                MaintenanceId,
                It.IsAny<MaintenanceDTOAPI>()))
            .ReturnsAsync(maintenance);

        using var client = CreateClient("ADMIN");
        using var content = CreateJsonContent(maintenance);
        using var response = await client.PutAsync(
            $"/api/Maintenance/{MaintenanceId}",
            content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Put_WhenUserRole_ReturnsForbidden()
    {
        using var client = CreateClient("USER");
        using var content = CreateJsonContent(CreateMaintenance());
        using var response = await client.PutAsync(
            $"/api/Maintenance/{MaintenanceId}",
            content);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _factory.ServiceMock.Verify(
            service => service.UpdateAsync(
                It.IsAny<string>(),
                It.IsAny<MaintenanceDTOAPI>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_WhenAdminAndReasonProvided_ReturnsNoContent()
    {
        _factory.ServiceMock
            .Setup(service => service.DeleteAsync(
                MaintenanceId,
                DeleteReason))
            .ReturnsAsync(true);

        using var client = CreateClient("ADMIN");
        using var response = await client.DeleteAsync(
            $"/api/Maintenance/{MaintenanceId}?reason=" +
            Uri.EscapeDataString(DeleteReason));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        _factory.ServiceMock.Verify(
            service => service.DeleteAsync(
                MaintenanceId,
                DeleteReason),
            Times.Once);
    }

    [Fact]
    public async Task Delete_WhenReasonIsMissing_ReturnsBadRequest()
    {
        using var client = CreateClient("ADMIN");
        using var response = await client.DeleteAsync(
            $"/api/Maintenance/{MaintenanceId}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        _factory.ServiceMock.Verify(
            service => service.DeleteAsync(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_WhenUserRole_ReturnsForbidden()
    {
        using var client = CreateClient("USER");
        using var response = await client.DeleteAsync(
            $"/api/Maintenance/{MaintenanceId}?reason=" +
            Uri.EscapeDataString(DeleteReason));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _factory.ServiceMock.Verify(
            service => service.DeleteAsync(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_WhenServiceRejectsDeletion_ReturnsNotFound()
    {
        _factory.ServiceMock
            .Setup(service => service.DeleteAsync(
                MaintenanceId,
                DeleteReason))
            .ReturnsAsync(false);

        _factory.ServiceMock
            .Setup(service => service.GetByIdAsync(MaintenanceId))
            .ReturnsAsync((MaintenanceDTOAPI?)null);

        using var client = CreateClient("ADMIN");
        using var response = await client.DeleteAsync(
            $"/api/Maintenance/{MaintenanceId}?reason=" +
            Uri.EscapeDataString(DeleteReason));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAudit_WhenAdmin_ReturnsOk()
    {
        _factory.ServiceMock
            .Setup(service => service.GetAuditAsync(MaintenanceId))
            .ReturnsAsync(new List<MaintenanceAuditDB>());

        using var client = CreateClient("ADMIN");
        using var response = await client.GetAsync(
            $"/api/Maintenance/{MaintenanceId}/audit");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAudit_WhenUserRole_ReturnsForbidden()
    {
        using var client = CreateClient("USER");
        using var response = await client.GetAsync(
            $"/api/Maintenance/{MaintenanceId}/audit");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _factory.ServiceMock.Verify(
            service => service.GetAuditAsync(It.IsAny<string>()),
            Times.Never);
    }

    private HttpClient CreateClient(string role)
    {
        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                CreateTestToken(role));

        return client;
    }

    private static string CreateTestToken(string role)
    {
        var now = DateTime.UtcNow;

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(TestKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims:
            [
                new Claim(
                    ClaimTypes.NameIdentifier,
                    "507f1f77bcf86cd799439099"),
                new Claim(
                    ClaimTypes.Name,
                    "Maintenance Integration Test"),
                new Claim(ClaimTypes.Role, role)
            ],
            notBefore: now.AddSeconds(-5),
            expires: now.AddMinutes(10),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static MaintenanceDTOAPI CreateMaintenance()
    {
        return new MaintenanceDTOAPI
        {
            Id = MaintenanceId,
            BusId = "507f1f77bcf86cd799439012",
            BusNumber = "001",
            ServiceType = "Revisão",
            Description = "Revisão preventiva de teste",
            ServiceDate = DateTime.UtcNow.AddDays(2),
            Cost = 250m,
            Status = "Agendada"
        };
    }

    private static StringContent CreateJsonContent(
        MaintenanceDTOAPI maintenance)
    {
        return new StringContent(
            JsonConvert.SerializeObject(maintenance),
            Encoding.UTF8,
            "application/json");
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private sealed class MaintenanceTestFactory
        : WebApplicationFactory<Program>
    {
        public Mock<IMaintenanceServiceAPI> ServiceMock { get; } = new();

        public MaintenanceTestFactory()
        {
            Environment.SetEnvironmentVariable(
                "JwtSettings__SecretKey",
                TestKey);

            Environment.SetEnvironmentVariable(
                "JwtSettings__Issuer",
                TestIssuer);

            Environment.SetEnvironmentVariable(
                "JwtSettings__Audience",
                TestAudience);

            Environment.SetEnvironmentVariable(
                "JwtSettings__ExpirationInMinutes",
                "30");

            Environment.SetEnvironmentVariable(
                "BootstrapAdmin__Secret",
                null);
        }

        protected override void ConfigureWebHost(
            IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["JwtSettings:SecretKey"] = TestKey,
                        ["JwtSettings:Issuer"] = TestIssuer,
                        ["JwtSettings:Audience"] = TestAudience,
                        ["JwtSettings:ExpirationInMinutes"] = "30",
                        ["BootstrapAdmin:Secret"] = null
                    });
            });

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IMaintenanceServiceAPI>();

                services.AddSingleton<IMaintenanceServiceAPI>(
                    ServiceMock.Object);
            });
        }
    }
}
