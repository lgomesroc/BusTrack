using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.API.ControllersAPI;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UserPresenceController(IMongoDatabase database)
    : ControllerBase
{
    private readonly IMongoCollection<BsonDocument> _sessions =
        database.GetCollection<BsonDocument>("BusTrackActiveSessions");

    [HttpPost("presence")]
    public async Task<IActionResult> Update(
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var browserId = User.FindFirstValue("browser_id");
        var sessionId = User.FindFirstValue("sid");

        if (string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(browserId) ||
            string.IsNullOrWhiteSpace(sessionId))
        {
            return Unauthorized();
        }

        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("_id", browserId),
            Builders<BsonDocument>.Filter.Eq("sessionId", sessionId),
            Builders<BsonDocument>.Filter.Eq("userId", userId));

        var update = Builders<BsonDocument>.Update
            .Set("email", User.FindFirstValue(ClaimTypes.Email) ?? "")
            .Set("fullName", User.FindFirstValue(ClaimTypes.Name) ?? "")
            .Set("role", User.FindFirstValue(ClaimTypes.Role) ?? "USER")
            .Set("lastSeenUtc", DateTime.UtcNow);

        var result = await _sessions.UpdateOneAsync(
            filter,
            update,
            cancellationToken: cancellationToken);

        if (result.MatchedCount == 0)
        {
            return Unauthorized();
        }

        return NoContent();
    }

    [HttpDelete("presence")]
    public async Task<IActionResult> Clear(
        CancellationToken cancellationToken)
    {
        var browserId = User.FindFirstValue("browser_id");
        var sessionId = User.FindFirstValue("sid");

        if (string.IsNullOrWhiteSpace(browserId) ||
            string.IsNullOrWhiteSpace(sessionId))
        {
            return Unauthorized();
        }

        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("_id", browserId),
            Builders<BsonDocument>.Filter.Eq("sessionId", sessionId));

        await _sessions.DeleteOneAsync(filter, cancellationToken);

        return NoContent();
    }
}
