
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusTrack.BusTrack.DB.ClassesDB
{
    public class UserDB
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? FullName { get; set; }

        public string? CPF { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Login { get; set; }

        public string Role { get; set; } = "USER";

        public DateTime? CreatedAtUtc { get; set; }

        // Data da última alteração de senha.
        public DateTime? PasswordChangedAtUtc { get; set; }

        // Hashes das cinco senhas anteriores, da mais recente à mais antiga.
        public List<string> PasswordHistory { get; set; } = new();

        // Controle das solicitações de recuperação.
        public DateTime? RecoveryFirstWindowStartedAtUtc { get; set; }

        public int RecoveryFirstWindowRequests { get; set; }

        public DateTime? RecoverySecondWindowStartedAtUtc { get; set; }

        public int RecoverySecondWindowRequests { get; set; }

        // Suspensões e bloqueio administrativo da recuperação.
        public DateTime? RecoverySuspendedUntilUtc { get; set; }

        public int RecoveryOffenses { get; set; }

        public bool RecoveryLockedByAdmin { get; set; }

        public string? RecoveryUnlockJustification { get; set; }

        public DateTime? RecoveryUnlockedAtUtc { get; set; }

        public string? RecoveryUnlockedByUserId { get; set; }
    }
}
