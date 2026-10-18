using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BusTrack.BusTrack.API.ControllersAPI;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UserPresenceController(IMongoDatabase database) : ControllerBase
{
    [HttpPost("presence")]
    public async Task<IActionResult> Update(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var fullName = User.FindFirstValue(ClaimTypes.Name) ?? email;
        var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role") ?? "USER";
        var sessions = database.GetCollection<BsonDocument>("BusTrackActiveSessions");
        var filter = Builders<BsonDocument>.Filter.Eq("userId", userId);
        var update = Builders<BsonDocument>.Update
            .Set("userId", userId)
            .Set("email", email)
            .Set("fullName", fullName)
            .Set("role", role)
            .Set("lastSeenUtc", DateTime.UtcNow);
        await sessions.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true }, cancellationToken);
        return NoContent();
    }

    [HttpDelete("presence")]
    public async Task<IActionResult> Clear(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();
        await database.GetCollection<BsonDocument>("BusTrackActiveSessions")
            .DeleteOneAsync(Builders<BsonDocument>.Filter.Eq("userId", userId), cancellationToken);
        return NoContent();
    }
}