using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.API.ControllersAPI;

[ApiController]
[Route("api/[controller]")]
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
        var users = await accounts.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: cancellationToken);
        var admins = await accounts.CountDocumentsAsync(Builders<BsonDocument>.Filter.Eq("Role", "ADMIN"), cancellationToken: cancellationToken);
        var busCount = await buses.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: cancellationToken);
        var driverCount = await drivers.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: cancellationToken);
        var passengerCount = await passengers.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: cancellationToken);
        var routeCount = await routes.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: cancellationToken);
        var tripCount = await trips.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: cancellationToken);
        var upcoming = await trips.CountDocumentsAsync(Builders<BsonDocument>.Filter.Gte("DepartureTime", DateTime.UtcNow), cancellationToken: cancellationToken);
        return Ok(new { generatedAtUtc = DateTime.UtcNow, users = new { total = users, administrators = admins, normalUsers = users - admins }, buses = new { total = busCount }, drivers = new { total = driverCount }, passengers = new { total = passengerCount }, routes = new { total = routeCount }, trips = new { total = tripCount, upcoming } });
    }
}
