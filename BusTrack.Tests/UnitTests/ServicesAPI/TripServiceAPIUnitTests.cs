using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.Tests.MappingsIntegrationTests;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BusTrack.Tests.UnitTests.ServicesAPI;

public class TripServiceAPIUnitTests
{
    private const string TripId = "507f1f77bcf86cd799439011";
    private const string BusId = "507f1f77bcf86cd799439012";
    private const string DriverId = "507f1f77bcf86cd799439013";
    private const string RouteId = "507f1f77bcf86cd799439014";
    private const string PassengerId = "507f1f77bcf86cd799439015";

    private readonly Mock<ITripRepositoryDB> _tripRepository = new();
    private readonly Mock<ITripPassengerRepositoryDB> _tripPassengerRepository = new();
    private readonly Mock<IBusRepositoryDB> _busRepository = new();
    private readonly Mock<IDriverRepositoryDB> _driverRepository = new();
    private readonly Mock<IRouteRepositoryDB> _routeRepository = new();
    private readonly Mock<IPassengerRepositoryDB> _passengerRepository = new();

    private readonly TripServiceAPI _service;

    public TripServiceAPIUnitTests()
    {
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<AutoMapperProfile>(),
            NullLoggerFactory.Instance);

        _service = new TripServiceAPI(
            _tripRepository.Object,
            _tripPassengerRepository.Object,
            _busRepository.Object,
            _driverRepository.Object,
            _routeRepository.Object,
            _passengerRepository.Object,
            configuration.CreateMapper());
    }

    [Fact]
    public async Task GetAllTripDetailsAsync_WhenNoTrips_ReturnsEmptyCollection()
    {
        _tripRepository
            .Setup(repository => repository.GetAllTripsAsync())
            .ReturnsAsync(new List<TripDB>());

        var result = await _service.GetAllTripDetailsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);

        _tripRepository.Verify(
            repository => repository.GetAllTripsAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetAllTripDetailsAsync_WhenTripsExist_ReturnsDetailsForEveryTrip()
    {
        var trips = new List<TripDB>
        {
            new()
            {
                Id = TripId,
                DepartureTime = new DateTime(2026, 10, 20, 10, 0, 0),
                ArrivalTime = new DateTime(2026, 10, 20, 11, 0, 0),
                Duration = 60,
                LimitPassengers = 40
            },
            new()
            {
                Id = "507f1f77bcf86cd799439016",
                DepartureTime = new DateTime(2026, 10, 21, 10, 0, 0),
                ArrivalTime = new DateTime(2026, 10, 21, 12, 0, 0),
                Duration = 120,
                LimitPassengers = 45
            }
        };

        _tripRepository
            .Setup(repository => repository.GetAllTripsAsync())
            .ReturnsAsync(trips);

        SetupEmptyPassengerAssociations(TripId);
        SetupEmptyPassengerAssociations("507f1f77bcf86cd799439016");

        var result = (await _service.GetAllTripDetailsAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(TripId, result[0].Id);
        Assert.Equal(60, result[0].Duration);
        Assert.Equal("507f1f77bcf86cd799439016", result[1].Id);
        Assert.Equal(120, result[1].Duration);
    }

    [Fact]
    public async Task GetTripDetailsByIdAsync_WhenIdIsNull_ReturnsNullWithoutRepositoryCall()
    {
        var result = await _service.GetTripDetailsByIdAsync(null);

        Assert.Null(result);

        _tripRepository.Verify(
            repository => repository.GetTripByIdAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetTripDetailsByIdAsync_WhenIdIsBlank_ReturnsNullWithoutRepositoryCall(
        string id)
    {
        var result = await _service.GetTripDetailsByIdAsync(id);

        Assert.Null(result);

        _tripRepository.Verify(
            repository => repository.GetTripByIdAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task GetTripDetailsByIdAsync_WhenTripDoesNotExist_ReturnsNull()
    {
        _tripRepository
            .Setup(repository => repository.GetTripByIdAsync(TripId))
            .ReturnsAsync((TripDB?)null);

        var result = await _service.GetTripDetailsByIdAsync(TripId);

        Assert.Null(result);

        _tripRepository.Verify(
            repository => repository.GetTripByIdAsync(TripId),
            Times.Once);
    }

    [Fact]
    public async Task GetTripDetailsByIdAsync_WhenRelatedRecordsExist_ReturnsCompleteDetails()
    {
        var trip = new TripDB
        {
            Id = TripId,
            UserId = "507f1f77bcf86cd799439099",
            BusId = BusId,
            DriverId = DriverId,
            RouteId = RouteId,
            DepartureTime = new DateTime(2026, 10, 20, 10, 0, 0),
            ArrivalTime = new DateTime(2026, 10, 20, 11, 30, 0),
            Duration = 90,
            LimitPassengers = 40
        };

        _tripRepository
            .Setup(repository => repository.GetTripByIdAsync(TripId))
            .ReturnsAsync(trip);

        _busRepository
            .Setup(repository => repository.GetBusByIdAsync(BusId))
            .ReturnsAsync(new BusDB
            {
                Id = BusId,
                Number = "001",
                Plate = "ABC1234",
                Line = "Mercedes-Benz",
                Capacity = 40
            });

        _driverRepository
            .Setup(repository => repository.GetDriverByIdAsync(DriverId))
            .ReturnsAsync(new DriverDB
            {
                Id = DriverId,
                Name = "Motorista Teste",
                Cpf = "12345678900"
            });

        _routeRepository
            .Setup(repository => repository.GetRouteByIdAsync(RouteId))
            .ReturnsAsync(new RouteDB
            {
                Id = RouteId,
                Name = "Centro - Zona Sul",
                Origin = "Centro",
                Destination = "Zona Sul"
            });

        _tripPassengerRepository
            .Setup(repository => repository.GetByTripIdAsync(TripId))
            .ReturnsAsync(new List<TripPassengerDB>
            {
                new()
                {
                    TripId = TripId,
                    PassengerId = PassengerId
                }
            });

        _passengerRepository
            .Setup(repository => repository.GetPassengerByIdAsync(PassengerId))
            .ReturnsAsync(new PassengerDB
            {
                Id = PassengerId,
                Name = "Passageiro Teste",
                Cpf = "12345678900",
                Email = "passageiro@bustrack.local",
                Phone = "21999999999"
            });

        var result = await _service.GetTripDetailsByIdAsync(TripId);

        Assert.NotNull(result);
        Assert.Equal(TripId, result.Id);
        Assert.Equal(trip.UserId, result.UserId);
        Assert.Equal(trip.DepartureTime, result.DepartureTime);
        Assert.Equal(trip.ArrivalTime, result.ArrivalTime);
        Assert.Equal(90, result.Duration);
        Assert.Equal(40, result.LimitPassengers);

        Assert.NotNull(result.Bus);
        Assert.Equal(BusId, result.Bus.Id);
        Assert.Equal("001", result.Bus.Number);
        Assert.Equal("ABC1234", result.Bus.LicensePlate);
        Assert.Equal("Mercedes-Benz", result.Bus.Model);
        Assert.Equal(40, result.Bus.Capacity);

        Assert.NotNull(result.Driver);
        Assert.Equal(DriverId, result.Driver.Id);
        Assert.Equal("Motorista Teste", result.Driver.Name);
        Assert.Equal("12345678900", result.Driver.LicenseNumber);

        Assert.NotNull(result.Route);
        Assert.Equal(RouteId, result.Route.Id);
        Assert.Equal("Centro - Zona Sul", result.Route.Name);
        Assert.Equal("Centro", result.Route.Origin);
        Assert.Equal("Zona Sul", result.Route.Destination);

        var passenger = Assert.Single(result.Passengers);
        Assert.Equal(PassengerId, passenger.Id);
        Assert.Equal("Passageiro Teste", passenger.Name);
        Assert.Equal("12345678900", passenger.Cpf);
        Assert.Equal("passageiro@bustrack.local", passenger.Email);
        Assert.Equal("21999999999", passenger.Phone);
    }

    [Fact]
    public async Task GetTripDetailsByIdAsync_WhenRelatedIdsAreMissing_ReturnsTripWithoutRelatedDetails()
    {
        var trip = new TripDB
        {
            Id = TripId,
            DepartureTime = new DateTime(2026, 10, 20, 10, 0, 0),
            ArrivalTime = new DateTime(2026, 10, 20, 11, 0, 0),
            Duration = 60,
            LimitPassengers = 40,
            BusId = null,
            DriverId = "",
            RouteId = null
        };

        _tripRepository
            .Setup(repository => repository.GetTripByIdAsync(TripId))
            .ReturnsAsync(trip);

        SetupEmptyPassengerAssociations(TripId);

        var result = await _service.GetTripDetailsByIdAsync(TripId);

        Assert.NotNull(result);
        Assert.Equal(TripId, result.Id);
        Assert.Null(result.Bus);
        Assert.Null(result.Driver);
        Assert.Null(result.Route);
        Assert.Empty(result.Passengers);

        _busRepository.Verify(
            repository => repository.GetBusByIdAsync(It.IsAny<string>()),
            Times.Never);

        _driverRepository.Verify(
            repository => repository.GetDriverByIdAsync(It.IsAny<string>()),
            Times.Never);

        _routeRepository.Verify(
            repository => repository.GetRouteByIdAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task GetTripDetailsByIdAsync_WhenAssociationListIsEmpty_UsesPassengerIdsFromTrip()
    {
        var trip = new TripDB
        {
            Id = TripId,
            Passengers = new List<string> { PassengerId }
        };

        _tripRepository
            .Setup(repository => repository.GetTripByIdAsync(TripId))
            .ReturnsAsync(trip);

        SetupEmptyPassengerAssociations(TripId);

        _passengerRepository
            .Setup(repository => repository.GetPassengerByIdAsync(PassengerId))
            .ReturnsAsync(new PassengerDB
            {
                Id = PassengerId,
                Name = "Passageiro do Fallback"
            });

        var result = await _service.GetTripDetailsByIdAsync(TripId);

        Assert.NotNull(result);

        var passenger = Assert.Single(result.Passengers);
        Assert.Equal(PassengerId, passenger.Id);
        Assert.Equal("Passageiro do Fallback", passenger.Name);

        _passengerRepository.Verify(
            repository => repository.GetPassengerByIdAsync(PassengerId),
            Times.Once);
    }

    [Fact]
    public async Task GetTripDetailsByIdAsync_WhenPassengerAssociationHasDuplicateIds_ReturnsPassengerOnlyOnce()
    {
        var trip = new TripDB
        {
            Id = TripId
        };

        _tripRepository
            .Setup(repository => repository.GetTripByIdAsync(TripId))
            .ReturnsAsync(trip);

        _tripPassengerRepository
            .Setup(repository => repository.GetByTripIdAsync(TripId))
            .ReturnsAsync(new List<TripPassengerDB>
            {
                new() { TripId = TripId, PassengerId = PassengerId },
                new() { TripId = TripId, PassengerId = PassengerId }
            });

        _passengerRepository
            .Setup(repository => repository.GetPassengerByIdAsync(PassengerId))
            .ReturnsAsync(new PassengerDB
            {
                Id = PassengerId,
                Name = "Passageiro Único"
            });

        var result = await _service.GetTripDetailsByIdAsync(TripId);

        Assert.NotNull(result);
        Assert.Single(result.Passengers);

        _passengerRepository.Verify(
            repository => repository.GetPassengerByIdAsync(PassengerId),
            Times.Once);
    }

    [Fact]
    public async Task GetTripDetailsByIdAsync_WhenPassengerDoesNotExist_SkipsPassenger()
    {
        const string missingPassengerId = "507f1f77bcf86cd799439099";

        var trip = new TripDB
        {
            Id = TripId
        };

        _tripRepository
            .Setup(repository => repository.GetTripByIdAsync(TripId))
            .ReturnsAsync(trip);

        _tripPassengerRepository
            .Setup(repository => repository.GetByTripIdAsync(TripId))
            .ReturnsAsync(new List<TripPassengerDB>
            {
                new() { TripId = TripId, PassengerId = missingPassengerId }
            });

        _passengerRepository
            .Setup(repository => repository.GetPassengerByIdAsync(missingPassengerId))
            .ReturnsAsync((PassengerDB)null!);

        var result = await _service.GetTripDetailsByIdAsync(TripId);

        Assert.NotNull(result);
        Assert.Empty(result.Passengers);
    }

    private void SetupEmptyPassengerAssociations(string tripId)
    {
        _tripPassengerRepository
            .Setup(repository => repository.GetByTripIdAsync(tripId))
            .ReturnsAsync(new List<TripPassengerDB>());
    }
}
