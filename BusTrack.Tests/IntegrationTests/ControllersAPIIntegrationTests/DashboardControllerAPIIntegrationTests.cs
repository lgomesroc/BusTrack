using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace BusTrack.Tests.IntegrationTests.ControllersAPIIntegrationTests;

public sealed class DashboardControllerAPIIntegrationTests
    : IClassFixture<DashboardTestFactory>, IAsyncLifetime
{
    private const string TestKey =
        "BusTrack-test-only-signing-key-not-for-production-2026";
    private const string TestIssuer = "BusTrack.Tests";
    private const string TestAudience = "BusTrack.Tests.Client";

    private readonly DashboardTestFactory _factory;

    public DashboardControllerAPIIntegrationTests(DashboardTestFactory factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();

    public Task DisposeAsync() => _factory.ResetDatabaseAsync();

    [Fact]
    public async Task Admin_ReturnsResourceCountsUpcomingTripsAndActiveUsers()
    {
        var now = DateTime.UtcNow;
        var adminId = ObjectId.GenerateNewId();
        var userId = ObjectId.GenerateNewId();

        await _factory.Accounts.InsertManyAsync(new[]
        {
            Account(adminId, "Admin Teste", "admin@example.com", "ADMIN", now.AddDays(-2)),
            Account(userId, "Usuário Teste", "user@example.com", "USER", now.AddDays(-1))
        });

        await _factory.Database.GetCollection<BsonDocument>("Buses")
            .InsertOneAsync(new BsonDocument("Plate", "ABC1D23"));
        await _factory.Database.GetCollection<BsonDocument>("Drivers")
            .InsertOneAsync(new BsonDocument("FullName", "Motorista Teste"));
        await _factory.Database.GetCollection<BsonDocument>("Passengers")
            .InsertOneAsync(new BsonDocument("FullName", "Passageiro Teste"));
        await _factory.Database.GetCollection<BsonDocument>("Routes")
            .InsertOneAsync(new BsonDocument("Name", "Rota Teste"));

        await _factory.Database.GetCollection<BsonDocument>("Trips").InsertManyAsync(new[]
        {
            new BsonDocument("DepartureTime", new BsonDateTime(now.AddHours(2))),
            new BsonDocument("DepartureTime", new BsonDateTime(now.AddHours(-2)))
        });

        await _factory.Sessions.InsertOneAsync(Session(
            adminId.ToString(), "Admin Teste", "admin@example.com", "ADMIN",
            now.AddSeconds(-30)));

        using var client = CreateClient("ADMIN");
        using var response = await client.GetAsync("/api/Dashboard/admin");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = await ReadJsonAsync(response);
        var root = json.RootElement;

        Assert.Equal(2, root.GetProperty("users").GetProperty("total").GetInt32());
        Assert.Equal(1, root.GetProperty("users").GetProperty("administrators").GetInt32());
        Assert.Equal(1, root.GetProperty("users").GetProperty("normalUsers").GetInt32());
        Assert.Equal(1, root.GetProperty("buses").GetProperty("total").GetInt32());
        Assert.Equal(1, root.GetProperty("drivers").GetProperty("total").GetInt32());
        Assert.Equal(1, root.GetProperty("passengers").GetProperty("total").GetInt32());
        Assert.Equal(1, root.GetProperty("routes").GetProperty("total").GetInt32());
        Assert.Equal(2, root.GetProperty("trips").GetProperty("total").GetInt32());
        Assert.Equal(1, root.GetProperty("trips").GetProperty("upcoming").GetInt32());

        var activeUsers = root.GetProperty("activeUsers").EnumerateArray().ToArray();
        Assert.Single(activeUsers);
        Assert.Equal(adminId.ToString(), activeUsers[0].GetProperty("userId").GetString());
        Assert.Equal("Admin Teste", activeUsers[0].GetProperty("fullName").GetString());
    }

    [Fact]
    public async Task Users_ReturnsPaginatedAccountsAndOnlineStatus()
    {
        var now = DateTime.UtcNow;
        var firstId = ObjectId.GenerateNewId();
        var secondId = ObjectId.GenerateNewId();
        var thirdId = ObjectId.GenerateNewId();

        await _factory.Accounts.InsertManyAsync(new[]
        {
            Account(firstId, "Primeiro Usuário", "first@example.com", "USER", now.AddDays(-1)),
            Account(secondId, "Segundo Usuário", "second@example.com", "USER", now.AddDays(-2)),
            Account(thirdId, "Terceiro Usuário", "third@example.com", "ADMIN", now.AddDays(-3))
        });

        await _factory.Sessions.InsertManyAsync(new[]
        {
            Session(firstId.ToString(), "Primeiro Usuário", "first@example.com", "USER",
                now.AddSeconds(-30)),
            Session(secondId.ToString(), "Segundo Usuário", "second@example.com", "USER",
                now.AddMinutes(-3))
        });

        using var client = CreateClient("ADMIN");
        using var firstResponse = await client.GetAsync("/api/Dashboard/users?page=1&pageSize=2");

        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        using var firstJson = await ReadJsonAsync(firstResponse);
        var firstRoot = firstJson.RootElement;

        Assert.Equal(1, firstRoot.GetProperty("page").GetInt32());
        Assert.Equal(2, firstRoot.GetProperty("pageSize").GetInt32());
        Assert.Equal(3, firstRoot.GetProperty("total").GetInt32());
        Assert.Equal(2, firstRoot.GetProperty("totalPages").GetInt32());

        var firstPageUsers = firstRoot.GetProperty("users").EnumerateArray().ToArray();
        Assert.Equal(2, firstPageUsers.Length);
        Assert.Equal(firstId.ToString(), firstPageUsers[0].GetProperty("id").GetString());
        Assert.True(firstPageUsers[0].GetProperty("isOnline").GetBoolean());
        Assert.Equal("Online", firstPageUsers[0].GetProperty("status").GetString());
        Assert.Equal(secondId.ToString(), firstPageUsers[1].GetProperty("id").GetString());
        Assert.False(firstPageUsers[1].GetProperty("isOnline").GetBoolean());
        Assert.Equal("Offline", firstPageUsers[1].GetProperty("status").GetString());

        using var secondResponse = await client.GetAsync("/api/Dashboard/users?page=2&pageSize=2");
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);

        using var secondJson = await ReadJsonAsync(secondResponse);
        var secondRoot = secondJson.RootElement;
        Assert.Equal(2, secondRoot.GetProperty("page").GetInt32());
        Assert.Single(secondRoot.GetProperty("users").EnumerateArray());
        Assert.Equal(thirdId.ToString(),
            secondRoot.GetProperty("users")[0].GetProperty("id").GetString());
    }

    [Fact]
    public async Task Users_SearchMatchesNameEmailAndRoleLiterally()
    {
        var now = DateTime.UtcNow;

        await _factory.Accounts.InsertManyAsync(new[]
        {
            Account(ObjectId.GenerateNewId(), "Ana+Maria", "ana@example.com", "USER", now),
            Account(ObjectId.GenerateNewId(), "Bruno Silva", "bruno@example.com", "USER", now.AddMinutes(-1)),
            Account(ObjectId.GenerateNewId(), "Carla Souza", "carla@example.com", "ADMIN", now.AddMinutes(-2))
        });

        using var client = CreateClient("ADMIN");

        using var nameResponse = await client.GetAsync("/api/Dashboard/users?search=Ana%2B");
        Assert.Equal(HttpStatusCode.OK, nameResponse.StatusCode);
        using var nameJson = await ReadJsonAsync(nameResponse);
        Assert.Equal(1, nameJson.RootElement.GetProperty("total").GetInt32());
        Assert.Equal("Ana+Maria",
            nameJson.RootElement.GetProperty("users")[0].GetProperty("fullName").GetString());

        using var emailResponse = await client.GetAsync("/api/Dashboard/users?search=bruno%40example.com");
        Assert.Equal(HttpStatusCode.OK, emailResponse.StatusCode);
        using var emailJson = await ReadJsonAsync(emailResponse);
        Assert.Equal(1, emailJson.RootElement.GetProperty("total").GetInt32());
        Assert.Equal("bruno@example.com",
            emailJson.RootElement.GetProperty("users")[0].GetProperty("email").GetString());

        using var roleResponse = await client.GetAsync("/api/Dashboard/users?search=ADMIN");
        Assert.Equal(HttpStatusCode.OK, roleResponse.StatusCode);
        using var roleJson = await ReadJsonAsync(roleResponse);
        Assert.Equal(1, roleJson.RootElement.GetProperty("total").GetInt32());
        Assert.Equal("ADMIN",
            roleJson.RootElement.GetProperty("users")[0].GetProperty("role").GetString());
    }

    [Fact]
    public async Task Users_ClampsPageAndPageSizeToSupportedLimits()
    {
        await _factory.Accounts.InsertOneAsync(Account(
            ObjectId.GenerateNewId(), "Usuário", "usuario@example.com", "USER", DateTime.UtcNow));

        using var client = CreateClient("ADMIN");
        using var response = await client.GetAsync("/api/Dashboard/users?page=0&pageSize=500");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var json = await ReadJsonAsync(response);
        Assert.Equal(1, json.RootElement.GetProperty("page").GetInt32());
        Assert.Equal(100, json.RootElement.GetProperty("pageSize").GetInt32());
        Assert.Equal(1, json.RootElement.GetProperty("total").GetInt32());
    }

    [Fact]
    public async Task AdminEndpoint_RejectsAnonymousRequests()
    {
        using var client = _factory.CreateClient();
        using var response = await client.GetAsync("/api/Dashboard/admin");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DashboardEndpoints_RejectUserRole()
    {
        using var client = CreateClient("USER");

        using var adminResponse = await client.GetAsync("/api/Dashboard/admin");
        using var usersResponse = await client.GetAsync("/api/Dashboard/users");

        Assert.Equal(HttpStatusCode.Forbidden, adminResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, usersResponse.StatusCode);
    }

    private HttpClient CreateClient(string role)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CreateTestToken(role));
        return client;
    }

    private static string CreateTestToken(string role)
    {
        var now = DateTime.UtcNow;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, ObjectId.GenerateNewId().ToString()),
                new Claim(ClaimTypes.Name, "Dashboard Integration Test"),
                new Claim(ClaimTypes.Role, role)
            },
            notBefore: now.AddSeconds(-5),
            expires: now.AddMinutes(10),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static BsonDocument Account(
        ObjectId id,
        string fullName,
        string email,
        string role,
        DateTime createdAtUtc) =>
        new()
        {
            ["_id"] = id,
            ["FullName"] = fullName,
            ["Email"] = email,
            ["Role"] = role,
            ["CreatedAtUtc"] = new BsonDateTime(createdAtUtc)
        };

    private static BsonDocument Session(
        string userId,
        string fullName,
        string email,
        string role,
        DateTime lastSeenUtc) =>
        new()
        {
            ["userId"] = userId,
            ["fullName"] = fullName,
            ["email"] = email,
            ["role"] = role,
            ["lastSeenUtc"] = new BsonDateTime(lastSeenUtc)
        };

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content);
    }
}

public sealed class DashboardTestFactory : WebApplicationFactory<Program>
{
    public const string TestDatabaseName = "BusTrackDB_DashboardIntegrationTests";

    private const string TestKey =
        "BusTrack-test-only-signing-key-not-for-production-2026";
    private const string TestIssuer = "BusTrack.Tests";
    private const string TestAudience = "BusTrack.Tests.Client";

    public DashboardTestFactory()
    {
        // Program validates JWT settings before ConfigureWebHost runs.
        Environment.SetEnvironmentVariable("JwtSettings__SecretKey", TestKey);
        Environment.SetEnvironmentVariable("JwtSettings__Issuer", TestIssuer);
        Environment.SetEnvironmentVariable("JwtSettings__Audience", TestAudience);
        Environment.SetEnvironmentVariable("JwtSettings__ExpirationInMinutes", "30");
        Environment.SetEnvironmentVariable("BootstrapAdmin__Secret", null);
    }

    public IMongoDatabase Database => Services.GetRequiredService<IMongoDatabase>();

    public IMongoCollection<BsonDocument> Accounts =>
        Database.GetCollection<BsonDocument>("Accounts");

    public IMongoCollection<BsonDocument> Sessions =>
        Database.GetCollection<BsonDocument>("BusTrackActiveSessions");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            var mongoUri = Environment.GetEnvironmentVariable("BUSTRACK_TEST_MONGODB_URI")
                ?? "mongodb://localhost:27017";

            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = TestKey,
                ["JwtSettings:Issuer"] = TestIssuer,
                ["JwtSettings:Audience"] = TestAudience,
                ["JwtSettings:ExpirationInMinutes"] = "30",
                ["BootstrapAdmin:Secret"] = null,
                ["ConnectionStrings:BusTrackDBConnection"] = mongoUri,
                ["ConnectionStrings:DatabaseName"] = TestDatabaseName
            });
        });

        // Pin the database instance used by the app to the dedicated test database.
        // This prevents appsettings/environment configuration from selecting BusTrackDB.
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IMongoDatabase>();
            services.AddSingleton<IMongoDatabase>(serviceProvider =>
            {
                var client = serviceProvider.GetRequiredService<IMongoClient>();
                return client.GetDatabase(TestDatabaseName);
            });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        if (!string.Equals(
                Database.DatabaseNamespace.DatabaseName,
                TestDatabaseName,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Refusing to delete unexpected database '{Database.DatabaseNamespace.DatabaseName}'.");
        }

        var client = Services.GetRequiredService<IMongoClient>();
        await client.DropDatabaseAsync(TestDatabaseName);
    }
}
