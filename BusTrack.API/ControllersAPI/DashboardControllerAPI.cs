
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace BusTrack.BusTrack.API.ControllersAPI;

[ApiController]
[Route("api/Dashboard")]
[Authorize(Roles = "ADMIN")]
public sealed class DashboardControllerAPI(IMongoDatabase database) : ControllerBase
{
    private static readonly TimeSpan OnlineWindow = TimeSpan.FromSeconds(90);

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

        var users = await accounts.CountDocumentsAsync(
            FilterDefinition<BsonDocument>.Empty,
            cancellationToken: cancellationToken);

        var admins = await accounts.CountDocumentsAsync(
            Builders<BsonDocument>.Filter.Eq("Role", "ADMIN"),
            cancellationToken: cancellationToken);

        var busCount = await buses.CountDocumentsAsync(
            FilterDefinition<BsonDocument>.Empty,
            cancellationToken: cancellationToken);

        var driverCount = await drivers.CountDocumentsAsync(
            FilterDefinition<BsonDocument>.Empty,
            cancellationToken: cancellationToken);

        var passengerCount = await passengers.CountDocumentsAsync(
            FilterDefinition<BsonDocument>.Empty,
            cancellationToken: cancellationToken);

        var routeCount = await routes.CountDocumentsAsync(
            FilterDefinition<BsonDocument>.Empty,
            cancellationToken: cancellationToken);

        var tripCount = await trips.CountDocumentsAsync(
            FilterDefinition<BsonDocument>.Empty,
            cancellationToken: cancellationToken);

        var upcoming = await trips.CountDocumentsAsync(
            Builders<BsonDocument>.Filter.Gte("DepartureTime", now),
            cancellationToken: cancellationToken);

        var activeFilter = Builders<BsonDocument>.Filter.Gte(
            "lastSeenUtc",
            now.Subtract(OnlineWindow));

        var activeDocuments = await sessions.Find(activeFilter)
            .SortByDescending(x => x["lastSeenUtc"])
            .ToListAsync(cancellationToken);

        var activeUsers = activeDocuments.Select(x => new
        {
            userId = GetString(x, "userId"),
            fullName = GetString(x, "fullName"),
            email = GetString(x, "email"),
            role = GetString(x, "role", "USER"),
            lastSeenUtc = GetDate(x, "lastSeenUtc")
        });

        return Ok(new
        {
            generatedAtUtc = now,
            users = new
            {
                total = users,
                administrators = admins,
                normalUsers = Math.Max(0, users - admins)
            },
            buses = new { total = busCount },
            drivers = new { total = driverCount },
            passengers = new { total = passengerCount },
            routes = new { total = routeCount },
            trips = new { total = tripCount, upcoming },
            activeUsers
        });
    }

    [HttpGet("users")]
    public async Task<IActionResult> Users(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var accounts = database.GetCollection<BsonDocument>("Accounts");
        var sessions = database.GetCollection<BsonDocument>("BusTrackActiveSessions");

        var filter = Builders<BsonDocument>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var escapedSearch = Regex.Escape(search.Trim());
            var searchFilter = Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Regex(
                    "FullName",
                    new BsonRegularExpression(escapedSearch, "i")),
                Builders<BsonDocument>.Filter.Regex(
                    "Email",
                    new BsonRegularExpression(escapedSearch, "i")),
                Builders<BsonDocument>.Filter.Regex(
                    "Role",
                    new BsonRegularExpression(escapedSearch, "i")));

            filter &= searchFilter;
        }

        var total = await accounts.CountDocumentsAsync(
            filter,
            cancellationToken: cancellationToken);

        var documents = await accounts.Find(filter)
            .SortByDescending(x => x["CreatedAtUtc"])
            .ThenBy(x => x["FullName"])
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var onlineCutoff = now.Subtract(OnlineWindow);

        var userIds = documents
            .Select(x => GetString(x, "_id"))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        var sessionFilter = Builders<BsonDocument>.Filter.In("userId", userIds);

        var sessionDocuments = userIds.Count == 0
            ? new List<BsonDocument>()
            : await sessions.Find(sessionFilter).ToListAsync(cancellationToken);

        var sessionsByUserId = sessionDocuments
            .Where(x => !string.IsNullOrWhiteSpace(GetString(x, "userId")))
            .GroupBy(x => GetString(x, "userId"))
            .ToDictionary(
                group => group.Key,
                group => group
                    .OrderByDescending(x => GetDate(x, "lastSeenUtc"))
                    .First());

        var result = documents.Select(user =>
        {
            var id = GetString(user, "_id");
            sessionsByUserId.TryGetValue(id, out var session);

            var lastSeenUtc = session is null
                ? null
                : GetDate(session, "lastSeenUtc");

            var isOnline = lastSeenUtc.HasValue &&
                           lastSeenUtc.Value >= onlineCutoff;

            return new
            {
                id,
                fullName = GetString(user, "FullName"),
                email = GetString(user, "Email"),
                role = GetString(user, "Role", "USER"),
                createdAtUtc = GetDate(user, "CreatedAtUtc"),
                lastSeenUtc,
                isOnline,
                status = isOnline ? "Online" : "Offline"
            };
        });

        return Ok(new
        {
            generatedAtUtc = now,
            page,
            pageSize,
            total,
            totalPages = (int)Math.Ceiling(total / (double)pageSize),
            users = result
        });
    }

    private static string GetString(
        BsonDocument document,
        string field,
        string defaultValue = "")
    {
        if (!document.TryGetValue(field, out var value) ||
            value.IsBsonNull)
        {
            return defaultValue;
        }

        return value.ToString() ?? defaultValue;
    }

    private static DateTime? GetDate(
        BsonDocument document,
        string field)
    {
        if (!document.TryGetValue(field, out var value) ||
            value.IsBsonNull)
        {
            return null;
        }

        if (value.IsBsonDateTime)
        {
            return value.AsBsonDateTime.ToUniversalTime();
        }

        if (value.IsString &&
            DateTime.TryParse(
                value.AsString,
                null,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out var parsed))
        {
            return parsed.Kind == DateTimeKind.Utc
                ? parsed
                : parsed.ToUniversalTime();
        }

        return null;
    }
}
