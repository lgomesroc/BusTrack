using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BusTrack.Tests.IntegrationTests.CustomWebApplicationFactory
{
    public class CustomWebApplicationFactory<TProgram>
        : Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<TProgram>
        where TProgram : class
    {
        private const string TestKey =
            "BusTrack-test-only-signing-key-not-for-production-2026";

        private const string TestIssuer = "BusTrack.Tests";
        private const string TestAudience = "BusTrack.Tests.Client";

        public CustomWebApplicationFactory()
        {
            // Program.Main validates JWT configuration before ConfigureWebHost runs.
            // Seed process-level test settings before WebApplicationFactory starts the host.
            Environment.SetEnvironmentVariable("JwtSettings__SecretKey", TestKey);
            Environment.SetEnvironmentVariable("JwtSettings__Issuer", TestIssuer);
            Environment.SetEnvironmentVariable("JwtSettings__Audience", TestAudience);
            Environment.SetEnvironmentVariable("JwtSettings__ExpirationInMinutes", "30");
            Environment.SetEnvironmentVariable("BootstrapAdmin__Secret", null);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JwtSettings:SecretKey"] = TestKey,
                    ["JwtSettings:Issuer"] = TestIssuer,
                    ["JwtSettings:Audience"] = TestAudience,
                    ["JwtSettings:ExpirationInMinutes"] = "30",
                    ["BootstrapAdmin:Secret"] = null
                });
            });
        }

        public new HttpClient CreateClient()
        {
            var client = base.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CreateTestToken("ADMIN"));
            return client;
        }

        public HttpClient CreateUserClient()
        {
            var client = base.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", CreateTestToken("USER"));
            return client;
        }

        private static string CreateTestToken(string role)
        {
            var now = DateTime.UtcNow;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: TestIssuer,
                audience: TestAudience,
                claims: new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "000000000000000000000001"),
                    new Claim(ClaimTypes.Name, "BusTrack Integration Test"),
                    new Claim(ClaimTypes.Role, role)
                },
                notBefore: now,
                expires: now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
