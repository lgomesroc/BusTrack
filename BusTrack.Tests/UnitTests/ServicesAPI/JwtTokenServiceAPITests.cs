using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.ClassesDB;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BusTrack.Tests.UnitTests.ServicesAPI
{
    public class JwtTokenServiceAPITests
    {
        private const string TestKey =
            "BusTrack-unit-test-signing-key-not-for-production-2026";

        [Fact]
        public void CreateToken_ReturnsSignedTokenWithExpectedClaims()
        {
            var configuration = CreateConfiguration(TestKey);
            var service = new JwtTokenServiceAPI(configuration);
            var user = new UserDB
            {
                Id = "000000000000000000000001",
                Email = "user@example.com",
                FullName = "Test User"
            };

            var (accessToken, expiresAtUtc) = service.CreateToken(user);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "BusTrack.Tests",
                ValidateAudience = true,
                ValidAudience = "BusTrack.Tests.Client",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = new JwtSecurityTokenHandler().ValidateToken(
                accessToken,
                validationParameters,
                out var validatedToken);

            Assert.IsType<JwtSecurityToken>(validatedToken);
            Assert.Equal("user@example.com", principal.FindFirst(ClaimTypes.Email)?.Value);
            Assert.Equal("000000000000000000000001",
                principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Assert.True(expiresAtUtc > DateTime.UtcNow);
        }

        [Fact]
        public void CreateToken_RejectsMissingSigningKey()
        {
            var configuration = CreateConfiguration(string.Empty);
            var service = new JwtTokenServiceAPI(configuration);

            Assert.Throws<InvalidOperationException>(() =>
                service.CreateToken(new UserDB
                {
                    Id = "000000000000000000000001",
                    Email = "user@example.com"
                }));
        }

        private static IConfiguration CreateConfiguration(string key) =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JwtSettings:SecretKey"] = key,
                    ["JwtSettings:Issuer"] = "BusTrack.Tests",
                    ["JwtSettings:Audience"] = "BusTrack.Tests.Client",
                    ["JwtSettings:ExpirationInMinutes"] = "30"
                })
                .Build();
    }
}
