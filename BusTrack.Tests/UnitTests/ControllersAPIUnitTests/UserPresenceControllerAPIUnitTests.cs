using System.Reflection;
using System.Security.Claims;
using BusTrack.BusTrack.API.ControllersAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;

namespace BusTrack.Tests.UnitTests.ControllersAPIUnitTests;

public class UserPresenceControllerAPIUnitTests
{
    private static UserPresenceController CreateController(
        ClaimsPrincipal? user = null)
    {
        var database = new Mock<IMongoDatabase>();

        var controller = new UserPresenceController(database.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user ?? new ClaimsPrincipal(
                        new ClaimsIdentity())
                }
            }
        };

        return controller;
    }

    [Fact]
    public void Controller_RequiresAuthentication()
    {
        var attribute = typeof(UserPresenceController)
            .GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(attribute);
        Assert.Null(attribute.Roles);
        Assert.Null(attribute.Policy);
    }

    [Fact]
    public void Controller_HasExpectedRoute()
    {
        var attribute = typeof(UserPresenceController)
            .GetCustomAttribute<RouteAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("api/[controller]", attribute.Template);
    }

    [Fact]
    public void UpdateAction_HasExpectedHttpPostRoute()
    {
        var method = typeof(UserPresenceController)
            .GetMethod(nameof(UserPresenceController.Update));

        Assert.NotNull(method);

        var attribute = method!.GetCustomAttribute<HttpPostAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("presence", attribute.Template);
    }

    [Fact]
    public void ClearAction_HasExpectedHttpDeleteRoute()
    {
        var method = typeof(UserPresenceController)
            .GetMethod(nameof(UserPresenceController.Clear));

        Assert.NotNull(method);

        var attribute = method!.GetCustomAttribute<HttpDeleteAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("presence", attribute.Template);
    }

    [Fact]
    public async Task Update_WithoutUserId_ReturnsUnauthorized()
    {
        var controller = CreateController();

        var result = await controller.Update(CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Clear_WithoutUserId_ReturnsUnauthorized()
    {
        var controller = CreateController();

        var result = await controller.Clear(CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void UpdateAction_IsAsynchronous()
    {
        var method = typeof(UserPresenceController)
            .GetMethod(nameof(UserPresenceController.Update));

        Assert.NotNull(method);
        Assert.Equal(typeof(Task<IActionResult>), method!.ReturnType);
    }

    [Fact]
    public void ClearAction_IsAsynchronous()
    {
        var method = typeof(UserPresenceController)
            .GetMethod(nameof(UserPresenceController.Clear));

        Assert.NotNull(method);
        Assert.Equal(typeof(Task<IActionResult>), method!.ReturnType);
    }
}
