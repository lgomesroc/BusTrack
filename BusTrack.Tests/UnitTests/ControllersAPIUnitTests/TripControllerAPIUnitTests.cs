using System.Security.Claims;
using BusTrack.BusTrack.API.ControllersAPI;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.ClassesDB;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BusTrack.Tests.UnitTests.ControllersAPIUnitTests.TripControllerAPIUnitTests;

public class TripControllerAPIUnitTests
{
    private const string UserId = "507f1f77bcf86cd799439099";
    private const string PassengerId = "507f1f77bcf86cd799439021";
    private const string OtherPassengerId = "507f1f77bcf86cd799439022";

    private readonly Mock<ITripServiceAPI> _tripService;
    private readonly Mock<IUserRepositoryDB> _userRepository;
    private readonly Mock<IPassengerRepositoryDB> _passengerRepository;
    private readonly TripControllerAPI _controller;

    public TripControllerAPIUnitTests()
    {
        _tripService = new Mock<ITripServiceAPI>();
        _userRepository = new Mock<IUserRepositoryDB>();
        _passengerRepository = new Mock<IPassengerRepositoryDB>();

        _controller = new TripControllerAPI(
            _tripService.Object,
            _userRepository.Object,
            _passengerRepository.Object);

        SetUser(UserId, "teste@bustrack.local");
    }

    private void SetUser(string? userId, string? email = null, string? subject = null)
    {
        var claims = new List<Claim>();

        if (userId is not null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
        }

        if (email is not null)
        {
            claims.Add(new Claim(ClaimTypes.Email, email));
        }

        if (subject is not null)
        {
            claims.Add(new Claim("sub", subject));
        }

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "UnitTest"))
            }
        };
    }

    private static TripDetailsDTOAPI CreateTrip(
        string id,
        string? userId = null,
        params string[] passengerIds)
    {
        return new TripDetailsDTOAPI
        {
            Id = id,
            UserId = userId,
            DepartureTime = new DateTime(2026, 10, 20, 10, 0, 0),
            ArrivalTime = new DateTime(2026, 10, 20, 11, 0, 0),
            Duration = 60,
            LimitPassengers = 40,
            Passengers = passengerIds
                .Select(passengerId => new PassengerDetailsDTOAPI
                {
                    Id = passengerId
                })
                .ToList()
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        var trips = new List<TripDetailsDTOAPI>
        {
            CreateTrip("trip-1")
        };

        _tripService
            .Setup(service => service.GetAllTripDetailsAsync())
            .ReturnsAsync(trips);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(trips, okResult.Value);
    }

    [Fact]
    public async Task GetMine_WithoutUserIdOrSubject_ReturnsUnauthorized()
    {
        SetUser(null, "teste@bustrack.local");

        var result = await _controller.GetMine();

        Assert.IsType<UnauthorizedResult>(result);

        _tripService.Verify(
            service => service.GetAllTripDetailsAsync(),
            Times.Never);
    }

    [Fact]
    public async Task GetMine_IncludesOwnTripsAndExcludesUnrelatedTrips()
    {
        var ownTrip = CreateTrip("trip-own", UserId);
        var otherTrip = CreateTrip("trip-other", "another-user");

        _tripService
            .Setup(service => service.GetAllTripDetailsAsync())
            .ReturnsAsync(new List<TripDetailsDTOAPI>
            {
                ownTrip,
                otherTrip
            });

        _userRepository
            .Setup(repository => repository.GetByEmail("teste@bustrack.local"))
            .Returns((UserDB?)null);

        var result = await _controller.GetMine();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var trips = Assert.IsAssignableFrom<IEnumerable<TripDetailsDTOAPI>>(
            okResult.Value);

        Assert.Single(trips);
        Assert.Equal("trip-own", Assert.Single(trips).Id);

        _passengerRepository.Verify(
            repository => repository.GetAllPassengersAsync(),
            Times.Never);
    }

    [Fact]
    public async Task GetMine_IncludesTripsLinkedByEmail()
    {
        const string email = "teste@bustrack.local";

        _userRepository
            .Setup(repository => repository.GetByEmail(email))
            .Returns(new UserDB
            {
                Id = UserId,
                Email = email,
                CPF = "123.456.789-00"
            });

        _passengerRepository
            .Setup(repository => repository.GetAllPassengersAsync())
            .ReturnsAsync((IEnumerable<PassengerDB>)new List<PassengerDB>
            {
                new()
                {
                    Id = PassengerId,
                    Email = "TESTE@BUSTRACK.LOCAL",
                    Cpf = "999.999.999-99"
                }
            });

        var linkedTrip = CreateTrip("trip-passenger", "another-user", PassengerId);
        var unrelatedTrip = CreateTrip("trip-unrelated", "another-user", OtherPassengerId);

        _tripService
            .Setup(service => service.GetAllTripDetailsAsync())
            .ReturnsAsync(new List<TripDetailsDTOAPI>
            {
                linkedTrip,
                unrelatedTrip
            });

        var result = await _controller.GetMine();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var trips = Assert.IsAssignableFrom<IEnumerable<TripDetailsDTOAPI>>(
            okResult.Value);

        var trip = Assert.Single(trips);
        Assert.Equal("trip-passenger", trip.Id);

        _passengerRepository.Verify(
            repository => repository.GetAllPassengersAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetMine_IncludesTripsLinkedByNormalizedCpf()
    {
        const string email = "teste@bustrack.local";

        _userRepository
            .Setup(repository => repository.GetByEmail(email))
            .Returns(new UserDB
            {
                Id = UserId,
                Email = email,
                CPF = "123.456.789-00"
            });

        _passengerRepository
            .Setup(repository => repository.GetAllPassengersAsync())
            .ReturnsAsync((IEnumerable<PassengerDB>)new List<PassengerDB>
            {
                new()
                {
                    Id = PassengerId,
                    Email = "outro@example.com",
                    Cpf = "12345678900"
                }
            });

        var linkedTrip = CreateTrip("trip-cpf", "another-user", PassengerId);

        _tripService
            .Setup(service => service.GetAllTripDetailsAsync())
            .ReturnsAsync(new List<TripDetailsDTOAPI>
            {
                linkedTrip
            });

        var result = await _controller.GetMine();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var trips = Assert.IsAssignableFrom<IEnumerable<TripDetailsDTOAPI>>(
            okResult.Value);

        Assert.Equal("trip-cpf", Assert.Single(trips).Id);
    }

    [Fact]
    public async Task GetMine_UsesSubClaimWhenNameIdentifierIsMissing()
    {
        SetUser(null, null, UserId);

        var ownTrip = CreateTrip("trip-sub", UserId);

        _tripService
            .Setup(service => service.GetAllTripDetailsAsync())
            .ReturnsAsync(new List<TripDetailsDTOAPI>
            {
                ownTrip,
                CreateTrip("trip-other", "another-user")
            });

        var result = await _controller.GetMine();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var trips = Assert.IsAssignableFrom<IEnumerable<TripDetailsDTOAPI>>(
            okResult.Value);

        Assert.Equal("trip-sub", Assert.Single(trips).Id);
    }

    [Fact]
    public async Task GetMine_WhenNoTripsMatch_ReturnsEmptyCollection()
    {
        _userRepository
            .Setup(repository => repository.GetByEmail("teste@bustrack.local"))
            .Returns((UserDB?)null);

        _tripService
            .Setup(service => service.GetAllTripDetailsAsync())
            .ReturnsAsync(new List<TripDetailsDTOAPI>
            {
                CreateTrip("trip-other", "another-user")
            });

        var result = await _controller.GetMine();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var trips = Assert.IsAssignableFrom<IEnumerable<TripDetailsDTOAPI>>(
            okResult.Value);

        Assert.Empty(trips);
    }

    [Fact]
    public async Task GetById_WhenExists_ReturnsOkResult()
    {
        const string id = "507f1f77bcf86cd799439011";
        var trip = CreateTrip(id);

        _tripService
            .Setup(service => service.GetTripDetailsByIdAsync(id))
            .ReturnsAsync(trip);

        var result = await _controller.GetById(id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(trip, okResult.Value);
    }

    [Fact]
    public async Task GetById_WhenNotExists_ReturnsNotFound()
    {
        const string id = "507f1f77bcf86cd799439011";

        _tripService
            .Setup(service => service.GetTripDetailsByIdAsync(id))
            .ReturnsAsync((TripDetailsDTOAPI?)null);

        var result = await _controller.GetById(id);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_WhenSuccessful_ReturnsCreatedAtRouteResult()
    {
        var trip = new TripDTOAPI
        {
            BusId = "507f1f77bcf86cd799439012",
            DriverId = "507f1f77bcf86cd799439013",
            RouteId = "507f1f77bcf86cd799439014",
            DepartureTime = new DateTime(2026, 9, 16, 16, 0, 0)
        };

        var createdTrip = new TripDTOAPI
        {
            Id = "507f1f77bcf86cd799439011",
            BusId = trip.BusId,
            DriverId = trip.DriverId,
            RouteId = trip.RouteId,
            DepartureTime = trip.DepartureTime
        };

        _tripService
            .Setup(service => service.CreateTripAsync(trip))
            .ReturnsAsync(createdTrip);

        var result = await _controller.Create(trip);

        var createdResult = Assert.IsType<CreatedAtRouteResult>(result);

        Assert.Equal("GetTrip", createdResult.RouteName);
        Assert.Equal(createdTrip, createdResult.Value);
        Assert.Equal(UserId, trip.UserId);
    }

    [Fact]
    public async Task Create_WhenBodyIsNull_ReturnsBadRequest()
    {
        var result = await _controller.Create(null!);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Update_WhenSuccessful_ReturnsOkResult()
    {
        const string id = "507f1f77bcf86cd799439011";

        var trip = new TripDTOAPI
        {
            BusId = "507f1f77bcf86cd799439012",
            DriverId = "507f1f77bcf86cd799439013",
            RouteId = "507f1f77bcf86cd799439014"
        };

        var updatedTrip = new TripDTOAPI
        {
            Id = id,
            BusId = trip.BusId,
            DriverId = trip.DriverId,
            RouteId = trip.RouteId
        };

        _tripService
            .Setup(service => service.UpdateTripAsync(id, trip))
            .ReturnsAsync(updatedTrip);

        var result = await _controller.Update(id, trip);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updatedTrip, okResult.Value);
    }

    [Fact]
    public async Task Update_WhenTripDoesNotExist_ReturnsNotFound()
    {
        const string id = "507f1f77bcf86cd799439011";

        var trip = new TripDTOAPI
        {
            BusId = "507f1f77bcf86cd799439012"
        };

        _tripService
            .Setup(service => service.UpdateTripAsync(id, trip))
            .ReturnsAsync((TripDTOAPI?)null);

        var result = await _controller.Update(id, trip);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Update_WhenBodyIsNull_ReturnsBadRequest()
    {
        const string id = "507f1f77bcf86cd799439011";

        var result = await _controller.Update(id, null!);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Delete_WhenSuccessful_ReturnsNoContent()
    {
        const string id = "507f1f77bcf86cd799439011";

        _tripService
            .Setup(service => service.DeleteTripAsync(id))
            .ReturnsAsync(true);

        var result = await _controller.Delete(id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_WhenTripDoesNotExist_ReturnsNotFound()
    {
        const string id = "507f1f77bcf86cd799439011";

        _tripService
            .Setup(service => service.DeleteTripAsync(id))
            .ReturnsAsync(false);

        var result = await _controller.Delete(id);

        Assert.IsType<NotFoundResult>(result);
    }
}
