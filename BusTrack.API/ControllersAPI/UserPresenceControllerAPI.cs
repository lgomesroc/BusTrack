using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.API.ControllersAPI;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UserPresenceControllerAPI(IMongoDatabase database) : ControllerBase
{
    private readonly IMongoCollection<BsonDocument> _sessions =
        database.GetCollection<BsonDocument>("BusTrackActiveSessions");

    [HttpPost("presence")]
    public async Task<IActionResult> Heartbeat(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var now = DateTime.UtcNow;
        var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
        var fullName = User.FindFirst(ClaimTypes.Name)?.Value ?? "";
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "USER";
        var filter = Builders<BsonDocument>.Filter.Eq("userId", userId);
        var update = Builders<BsonDocument>.Update
            .Set("userId", userId)
            .Set("email", email)
            .Set("fullName", fullName)
            .Set("role", role)
            .Set("lastSeenUtc", now)
            .SetOnInsert("createdAtUtc", now);

        await _sessions.UpdateOneAsync(
            filter,
            update,
            new UpdateOptions { IsUpsert = true },
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("presence")]
    public async Task<IActionResult> EndSession(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        await _sessions.DeleteOneAsync(
            Builders<BsonDocument>.Filter.Eq("userId", userId),
            cancellationToken);
        return NoContent();
    }
}