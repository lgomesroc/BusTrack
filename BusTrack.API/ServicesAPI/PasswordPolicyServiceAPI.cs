
using BusTrack.BusTrack.DB.ClassesDB;
using BCryptNet = BCrypt.Net.BCrypt;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public static class PasswordPolicyServiceAPI
    {
        public const int MinimumLength = 8;
        public const int MinimumLowercase = 2;
        public const int MinimumUppercase = 2;
        public const int MinimumDigits = 2;
        public const int MinimumSpecialCharacters = 2;
        public const int PasswordHistoryLimit = 5;
        public const int PasswordValidityDays = 90;
        public const int PasswordGracePeriodDays = 7;

        public static readonly int[] ExpirationWarningDays =
        {
            10, 5, 3, 2, 1
        };

        public static bool IsValid(string? password)
        {
            if (string.IsNullOrEmpty(password) ||
                password.Length < MinimumLength)
            {
                return false;
            }

            return password.Count(char.IsLower) >= MinimumLowercase
                && password.Count(char.IsUpper) >= MinimumUppercase
                && password.Count(char.IsDigit) >= MinimumDigits
                && password.Count(c => !char.IsLetterOrDigit(c)
                    && !char.IsWhiteSpace(c)) >= MinimumSpecialCharacters;
        }

        public static string GetValidationMessage()
        {
            return
                $"A senha deve ter pelo menos {MinimumLength} caracteres, " +
                $"incluindo no mínimo {MinimumLowercase} letras minúsculas, " +
                $"{MinimumUppercase} letras maiúsculas, " +
                $"{MinimumDigits} números e " +
                $"{MinimumSpecialCharacters} caracteres especiais.";
        }

        public static bool MatchesCurrentOrHistory(
            string newPassword,
            UserDB user)
        {
            if (!string.IsNullOrWhiteSpace(user.Password) &&
                BCryptNet.Verify(newPassword, user.Password))
            {
                return true;
            }

            return user.PasswordHistory.Any(hash =>
                !string.IsNullOrWhiteSpace(hash) &&
                BCryptNet.Verify(newPassword, hash));
        }

        public static void ApplyNewPassword(
            UserDB user,
            string newPassword,
            DateTime? nowUtc = null)
        {
            var now = nowUtc ?? DateTime.UtcNow;

            var history = user.PasswordHistory
                .Where(hash => !string.IsNullOrWhiteSpace(hash))
                .ToList();

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                history.Insert(0, user.Password);
            }

            user.PasswordHistory = history
                .Take(PasswordHistoryLimit)
                .ToList();

            user.Password = BCryptNet.HashPassword(newPassword);
            user.PasswordChangedAtUtc = now;
        }

        public static DateTime GetExpirationDate(UserDB user)
        {
            var changedAt = user.PasswordChangedAtUtc
                ?? user.CreatedAtUtc
                ?? DateTime.UtcNow;

            return changedAt.AddDays(PasswordValidityDays);
        }

        public static int GetDaysUntilExpiration(UserDB user, DateTime? nowUtc = null)
        {
            var now = nowUtc ?? DateTime.UtcNow;
            var remaining = GetExpirationDate(user) - now;

            return (int)Math.Ceiling(remaining.TotalDays);
        }

        public static bool IsPastGracePeriod(
            UserDB user,
            DateTime? nowUtc = null)
        {
            var now = nowUtc ?? DateTime.UtcNow;

            return now >
                GetExpirationDate(user).AddDays(PasswordGracePeriodDays);
        }

        public static bool IsExpirationWarningDue(
            UserDB user,
            DateTime? nowUtc = null)
        {
            var days = GetDaysUntilExpiration(user, nowUtc);

            return ExpirationWarningDays.Contains(days);
        }

        public static bool IsProtectedAdministrator(UserDB user)
        {
            return string.Equals(
                user.Email?.Trim(),
                "admin@admin.com",
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
