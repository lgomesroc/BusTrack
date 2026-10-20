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

        public (string AccessToken, DateTime ExpiresAtUtc) CreateToken(
            UserDB user,
            string browserId,
            string sessionId)
        {
            if (string.IsNullOrWhiteSpace(browserId) ||
                string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException(
                    "O identificador do navegador e da sessão é obrigatório.");
            }

            var key = _configuration["JwtSettings:SecretKey"];

            if (string.IsNullOrWhiteSpace(key) ||
                Encoding.UTF8.GetByteCount(key) < 32)
            {
                throw new InvalidOperationException(
                    "Configure JwtSettings__SecretKey com pelo menos 32 bytes.");
            }

            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];

            if (string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException(
                    "JwtSettings:Issuer e JwtSettings:Audience são obrigatórios.");
            }

            var lifetimeMinutes =
                _configuration.GetValue<int?>(
                    "JwtSettings:ExpirationInMinutes") ?? 30;

            if (lifetimeMinutes is < 1 or > 1440)
            {
                throw new InvalidOperationException(
                    "JwtSettings:ExpirationInMinutes deve estar entre 1 e 1440.");
            }

            if (string.IsNullOrWhiteSpace(user.Id))
            {
                throw new InvalidOperationException(
                    "O usuário precisa possuir um identificador.");
            }

            var now = DateTime.UtcNow;
            var expiresAtUtc = now.AddMinutes(lifetimeMinutes);

            var role = string.Equals(
                user.Role,
                "ADMIN",
                StringComparison.OrdinalIgnoreCase)
                    ? "ADMIN"
                    : "USER";

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(ClaimTypes.Role, role),
                new("browser_id", browserId),
                new("sid", sessionId)
            };

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            if (!string.IsNullOrWhiteSpace(user.FullName))
            {
                claims.Add(new Claim(ClaimTypes.Name, user.FullName));
            }

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return (
                new JwtSecurityTokenHandler().WriteToken(token),
                expiresAtUtc);
        }
    }
}
