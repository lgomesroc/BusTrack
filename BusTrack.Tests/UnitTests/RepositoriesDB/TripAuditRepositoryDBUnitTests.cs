using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.RepositoriesDB;
using MongoDB.Driver;

namespace BusTrack.Tests.UnitTests.RepositoriesDB
{
    public class TripAuditRepositoryDBUnitTests
    {
        [Fact]
        public void AuditModel_ShouldKeepRequiredFields()
        {
            var audit = new TripAuditDB
            {
                TripId = "507f1f77bcf86cd799439011",
                Action = "Created",
                PerformedBy = "admin",
                OccurredAtUtc = DateTime.UtcNow
            };

            Assert.Equal("507f1f77bcf86cd799439011", audit.TripId);
            Assert.Equal("Created", audit.Action);
            Assert.Equal("admin", audit.PerformedBy);
            Assert.NotEqual(default, audit.OccurredAtUtc);
        }
    }
}