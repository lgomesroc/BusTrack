using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.API.ControllersAPI;

[ApiController]
[Route("api/Dashboard")]
[Authorize(Roles = "ADMIN")]
public sealed class DashboardControllerAPI(IMongoDatabase database) : ControllerBase
{
    [HttpGet("admin")]
    public async Task<IActionResult> Admin(CancellationToken cancellationToken)
    {
        var accounts = database.GetCollection<BsonDocument>("Accounts");
        var buses = database.GetCollection<BsonDocument>("Buses");
        var drivers = database.GetCollection<BsonDocument>("Drivers");
        var passengers = database.GetCollection<BsonDocument>("Passengers");
        var routes = database.GetCollection<BsonDocument>("Routes");
        var trips = database.GetCollection<BsonDocument>("Trips");
        var sessions = database.GetCollection<BsonDocument>("BusTrackActiveSessions");
        var now = DateTime.UtcNow;

        var users = await accounts.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: cancellationToken);
        var admins = await accounts.CountDocumentsAsync(Builders<BsonDocument>.Filter.Eq("Role", "ADMIN"), cancellationToken: cancellationToken);
        var busCount = await buses.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: cancellationToken);
        var driverCount = await drivers.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: cancellationToken);
        var passengerCount = await passengers.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: cancellationToken);
        var routeCount = await routes.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: cancellationToken);
        var tripCount = await trips.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: cancellationToken);
        var upcoming = await trips.CountDocumentsAsync(Builders<BsonDocument>.Filter.Gte("DepartureTime", now), cancellationToken: cancellationToken);

        var activeFilter = Builders<BsonDocument>.Filter.Gte("lastSeenUtc", now.AddMinutes(-2));
        var activeDocuments = await sessions.Find(activeFilter)
            .SortByDescending(x => x["lastSeenUtc"])
            .ToListAsync(cancellationToken);
        var activeUsers = activeDocuments.Select(x => new
        {
            userId = x.GetValue("userId", "").ToString(),
            fullName = x.GetValue("fullName", "").ToString(),
            email = x.GetValue("email", "").ToString(),
            role = x.GetValue("role", "USER").ToString(),
            lastSeenUtc = x.GetValue("lastSeenUtc", BsonNull.Value).AsBsonDateTime.ToUniversalTime()
        });

        return Ok(new
        {
            generatedAtUtc = now,
            users = new { total = users, administrators = admins, normalUsers = Math.Max(0, users - admins) },
            buses = new { total = busCount },
            drivers = new { total = driverCount },
            passengers = new { total = passengerCount },
            routes = new { total = routeCount },
            trips = new { total = tripCount, upcoming },
            activeUsers
        });
    }
}