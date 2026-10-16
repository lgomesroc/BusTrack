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
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(key) || Encoding.UTF8.GetByteCount(key) < 32)
            {
                throw new InvalidOperationException(
                    "A configuração Jwt:Key deve conter pelo menos 32 bytes. Configure Jwt__Key no ambiente.");
            }

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException(
                    "As configurações Jwt:Issuer e Jwt:Audience são obrigatórias.");
            }

            var lifetimeMinutes = _configuration.GetValue<int?>("Jwt:AccessTokenMinutes") ?? 30;
            if (lifetimeMinutes is < 1 or > 1440)
            {
                throw new InvalidOperationException(
                    "Jwt:AccessTokenMinutes deve estar entre 1 e 1440 minutos.");
            }

            var now = DateTime.UtcNow;
            var expiresAtUtc = now.AddMinutes(lifetimeMinutes);
            var claims = new List<Claim>();

            if (!string.IsNullOrWhiteSpace(user.Id))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            }

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Email));
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
