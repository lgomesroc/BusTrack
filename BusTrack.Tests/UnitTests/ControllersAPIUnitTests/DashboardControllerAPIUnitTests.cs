using System.Reflection;
using BusTrack.BusTrack.API.ControllersAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.Tests.UnitTests.ControllersAPIUnitTests;

public class DashboardControllerAPIUnitTests
{
    [Fact]
    public void Controller_RequiresAdminRole()
    {
        var attribute = typeof(DashboardControllerAPI)
            .GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("ADMIN", attribute.Roles);
    }

    [Fact]
    public void Controller_HasExpectedRoute()
    {
        var attribute = typeof(DashboardControllerAPI)
            .GetCustomAttribute<RouteAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("api/Dashboard", attribute.Template);
    }

    [Fact]
    public void AdminAction_HasExpectedHttpGetRoute()
    {
        var method = typeof(DashboardControllerAPI)
            .GetMethod(nameof(DashboardControllerAPI.Admin));

        Assert.NotNull(method);

        var attribute = method!.GetCustomAttribute<HttpGetAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("admin", attribute.Template);
    }

    [Fact]
    public void UsersAction_HasExpectedHttpGetRoute()
    {
        var method = typeof(DashboardControllerAPI)
            .GetMethod(nameof(DashboardControllerAPI.Users));

        Assert.NotNull(method);

        var attribute = method!.GetCustomAttribute<HttpGetAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("users", attribute.Template);
    }

    [Fact]
    public void AdminAction_IsAsynchronous()
    {
        var method = typeof(DashboardControllerAPI)
            .GetMethod(nameof(DashboardControllerAPI.Admin));

        Assert.NotNull(method);
        Assert.Equal(typeof(Task<IActionResult>), method!.ReturnType);
    }

    [Fact]
    public void UsersAction_IsAsynchronous()
    {
        var method = typeof(DashboardControllerAPI)
            .GetMethod(nameof(DashboardControllerAPI.Users));

        Assert.NotNull(method);
        Assert.Equal(typeof(Task<IActionResult>), method!.ReturnType);
    }

    [Fact]
    public void UsersAction_HasExpectedQueryParameters()
    {
        var method = typeof(DashboardControllerAPI)
            .GetMethod(nameof(DashboardControllerAPI.Users));

        Assert.NotNull(method);

        var parameters = method!.GetParameters();

        Assert.Collection(
            parameters,
            parameter =>
            {
                Assert.Equal("search", parameter.Name);
                Assert.Equal(typeof(string), parameter.ParameterType);
                Assert.False(parameter.IsOptional);
            },
            parameter =>
            {
                Assert.Equal("page", parameter.Name);
                Assert.Equal(typeof(int), parameter.ParameterType);
                Assert.True(parameter.IsOptional);
                Assert.Equal(1, parameter.DefaultValue);
            },
            parameter =>
            {
                Assert.Equal("pageSize", parameter.Name);
                Assert.Equal(typeof(int), parameter.ParameterType);
                Assert.True(parameter.IsOptional);
                Assert.Equal(10, parameter.DefaultValue);
            },
            parameter =>
            {
                Assert.Equal("cancellationToken", parameter.Name);
                Assert.Equal(typeof(CancellationToken), parameter.ParameterType);
                Assert.True(parameter.IsOptional);
            });
    }
}
