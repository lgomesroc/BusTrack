
namespace BusTrack.BusTrack.API.DTOAPI
{
    public sealed class AuthenticationResponseDTOAPI
    {
        public bool Success { get; init; }

        public string AccessToken { get; init; } = string.Empty;

        public string TokenType { get; init; } = "Bearer";

        public DateTime ExpiresAtUtc { get; init; }

        public string WelcomeMessage { get; init; } = string.Empty;

        public DateTime PasswordExpiresAtUtc { get; init; }

        public int PasswordDaysUntilExpiration { get; init; }

        public bool PasswordExpirationWarning { get; init; }
    }
}
