using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.ClassesDB;
using Microsoft.IdentityModel.Tokens;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public sealed class JwtTokenServiceAPI : IJwtTokenServiceAPI
    {
        private readonly IConfiguration _configuration;

        public JwtTokenServiceAPI(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string AccessToken, DateTime ExpiresAtUtc) CreateToken(UserDB user)
        {
            var key = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrWhiteSpace(key) || Encoding.UTF8.GetByteCount(key) < 32)
            {
                throw new InvalidOperationException(
                    "Configure JwtSettings:SecretKey com pelo menos 32 bytes usando a variável de ambiente JwtSettings__SecretKey.");
            }

            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException(
                    "As configurações JwtSettings:Issuer e JwtSettings:Audience são obrigatórias.");
            }

            var lifetimeMinutes = _configuration.GetValue<int?>("JwtSettings:ExpirationInMinutes") ?? 30;
            if (lifetimeMinutes is < 1 or > 1440)
            {
                throw new InvalidOperationException(
                    "JwtSettings:ExpirationInMinutes deve estar entre 1 e 1440 minutos.");
            }

            var now = DateTime.UtcNow;
            var expiresAtUtc = now.AddMinutes(lifetimeMinutes);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, string.Equals(user.Role, "ADMIN", StringComparison.OrdinalIgnoreCase) ? "ADMIN" : "USER")
            };

            if (!string.IsNullOrWhiteSpace(user.Id))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            }

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            if (!string.IsNullOrWhiteSpace(user.FullName))
            {
                claims.Add(new Claim(ClaimTypes.Name, user.FullName));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
        }
    }
}
