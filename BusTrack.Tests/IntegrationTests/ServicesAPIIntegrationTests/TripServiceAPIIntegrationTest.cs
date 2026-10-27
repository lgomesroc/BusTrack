using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.Tests.MappingsIntegrationTests;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BusTrack.Tests.IntegrationTests.ServicesAPIIntegrationTests
{
    public class TripServiceAPIIntegrationTest
    {
        private readonly Mock<ITripRepositoryDB> _tripRepository = new();
        private readonly Mock<ITripPassengerRepositoryDB> _tripPassengerRepository = new();
        private readonly Mock<IBusRepositoryDB> _busRepository = new();
        private readonly Mock<IDriverRepositoryDB> _driverRepository = new();
        private readonly Mock<IRouteRepositoryDB> _routeRepository = new();
        private readonly Mock<IPassengerRepositoryDB> _passengerRepository = new();
        private readonly ITripServiceAPI _tripServiceAPI;

        public TripServiceAPIIntegrationTest()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>(), NullLoggerFactory.Instance);
            _tripServiceAPI = new TripServiceAPI(_tripRepository.Object, _tripPassengerRepository.Object, _busRepository.Object, _driverRepository.Object, _routeRepository.Object, _passengerRepository.Object, config.CreateMapper());
        }

        [Fact]
        public async Task GetAllTripsAsync_ReturnsAllTrips()
        {
            _tripRepository.Setup(r => r.GetAllTripsAsync()).ReturnsAsync(new List<TripDB> { new(), new() });
            var result = await _tripServiceAPI.GetAllTripsAsync();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _tripRepository.Verify(r => r.GetAllTripsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTripByIdAsync_WhenTripExists_ReturnsTrip()
        {
            const string id = "507f1f77bcf86cd799439011";
            var trip = new TripDB { Id = id, BusId = "507f1f77bcf86cd799439012", DriverId = "507f1f77bcf86cd799439013", RouteId = "507f1f77bcf86cd799439014" };
            _tripRepository.Setup(r => r.GetTripByIdAsync(id)).ReturnsAsync(trip);
            var result = await _tripServiceAPI.GetTripByIdAsync(id);
            Assert.NotNull(result);
            Assert.Equal(id, result!.Id);
            Assert.Equal(trip.BusId, result.BusId);
            Assert.Equal(trip.DriverId, result.DriverId);
            Assert.Equal(trip.RouteId, result.RouteId);
        }

        [Fact]
        public async Task GetTripByIdAsync_WhenTripDoesNotExist_ReturnsNull()
        {
            const string id = "507f1f77bcf86cd799439011";
            _tripRepository.Setup(r => r.GetTripByIdAsync(id)).ReturnsAsync((TripDB?)null);
            Assert.Null(await _tripServiceAPI.GetTripByIdAsync(id));
        }

        [Fact]
        public async Task CreateTripAsync_ReturnsCreatedTripAsScheduled()
        {
            var departure = DateTime.UtcNow.AddHours(1);
            var trip = new TripDTOAPI
            {
                BusId = "bus",
                DriverId = "driver",
                RouteId = "route",
                DepartureTime = departure,
                ArrivalTime = DateTime.UtcNow.AddHours(2),
                Duration = 60,
                LimitPassengers = 40,
                Passengers = new List<string>()
            };

            _tripRepository
                .Setup(r => r.AddTripAsync(It.IsAny<TripDB>()))
                .ReturnsAsync((TripDB db) =>
                {
                    db.Id = "trip";
                    return db;
                });

            var result = await _tripServiceAPI.CreateTripAsync(trip);

            Assert.NotNull(result);
            Assert.Equal("trip", result.Id);
            Assert.Equal(departure, result.DepartureTime);
            Assert.Equal("Agendada", result.Status);
            Assert.Null(result.StartedAt);
            Assert.Null(result.ArrivalTime);
            Assert.Equal(0, result.Duration);
            Assert.Equal(0, result.LimitPassengers);

            _tripRepository.Verify(
                r => r.AddTripAsync(It.IsAny<TripDB>()),
                Times.Once);
        }

            var result = await _tripServiceAPI.CreateTripAsync(trip);

            Assert.NotNull(result);
            Assert.Equal("trip", result.Id);
            Assert.Equal(departure, result.DepartureTime);
            Assert.Equal("Agendada", result.Status);
            Assert.Null(result.StartedAt);
            Assert.Null(result.ArrivalTime);
            Assert.Equal(0, result.Duration);
            Assert.Equal(0, result.LimitPassengers);
            _tripRepository.Verify(r => r.AddTripAsync(It.IsAny<TripDB>()), Times.Once);
        }

        [Fact]
        public async Task CreateTripAsync_WhenDepartureIsPast_Throws()
        {
            var trip = new TripDTOAPI
            {
                DepartureTime = DateTime.UtcNow.AddMinutes(-1)
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _tripServiceAPI.CreateTripAsync(trip));

            Assert.Equal(
                "A saída deve ser agendada para um horário futuro.",
                exception.Message);

            _tripRepository.Verify(
                r => r.AddTripAsync(It.IsAny<TripDB>()),
                Times.Never);
        }
        [Fact]
        public async Task UpdateTripAsync_AgendedToCancelled_ReturnsCancelledTrip()
        {
            const string id = "trip";
            var existing = new TripDB { Id = id, Status = "Agendada", DepartureTime = DateTime.UtcNow.AddHours(1), BusId = "bus", DriverId = "driver", RouteId = "route" };
            _tripRepository.Setup(r => r.GetTripByIdAsync(id)).ReturnsAsync(existing);
            _tripRepository.Setup(r => r.UpdateTripAsync(id, It.IsAny<TripDB>())).ReturnsAsync((string _, TripDB db) => db);

            var result = await _tripServiceAPI.UpdateTripAsync(id, new TripDTOAPI { Status = "Cancelada" });

            Assert.NotNull(result);
            Assert.Equal("Cancelada", result!.Status);
            Assert.Null(result.ArrivalTime);
            Assert.Equal(0, result.Duration);
            Assert.Equal(0, result.LimitPassengers);
            _tripRepository.Verify(r => r.UpdateTripAsync(id, It.IsAny<TripDB>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTripAsync_InProgressToCompleted_UsesExactDuration()
        {
            const string id = "trip";
            var started = DateTime.UtcNow.AddMinutes(-59);
            var arrival = DateTime.UtcNow.AddMinutes(-1);
            var existing = new TripDB { Id = id, Status = "Em andamento", StartedAt = started, DepartureTime = started, BusId = "bus", DriverId = "driver", RouteId = "route" };
            _tripRepository.Setup(r => r.GetTripByIdAsync(id)).ReturnsAsync(existing);
            _tripRepository.Setup(r => r.UpdateTripAsync(id, It.IsAny<TripDB>())).ReturnsAsync((string _, TripDB db) => db);

            var result = await _tripServiceAPI.UpdateTripAsync(id, new TripDTOAPI { Status = "Concluída", ArrivalTime = arrival, Duration = 58, LimitPassengers = 35 });

            Assert.NotNull(result);
            Assert.Equal("Concluída", result!.Status);
            Assert.Equal(58, result.Duration);
            Assert.Equal(35, result.LimitPassengers);
            Assert.Equal(arrival, result.ArrivalTime);
        }

        [Fact]
        public async Task UpdateTripAsync_WhenDurationDoesNotMatchArrival_Throws()
        {
            const string id = "trip";
            var started = DateTime.UtcNow.AddMinutes(-60);
            var arrival = DateTime.UtcNow.AddMinutes(-1);
            var existing = new TripDB { Id = id, Status = "Em andamento", StartedAt = started, DepartureTime = started };
            _tripRepository.Setup(r => r.GetTripByIdAsync(id)).ReturnsAsync(existing);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _tripServiceAPI.UpdateTripAsync(id, new TripDTOAPI { Status = "Concluída", ArrivalTime = arrival, Duration = 1, LimitPassengers = 35 }));
            Assert.Contains("duração informada deve corresponder", exception.Message);
        }

        [Fact]
        public async Task UpdateTripAsync_WhenCompletedTripIsChanged_Throws()
        {
            const string id = "trip";
            _tripRepository.Setup(r => r.GetTripByIdAsync(id)).ReturnsAsync(new TripDB { Id = id, Status = "Concluída" });
            await Assert.ThrowsAsync<ArgumentException>(() => _tripServiceAPI.UpdateTripAsync(id, new TripDTOAPI { Status = "Cancelada" }));
        }

        [Fact]
        public async Task UpdateTripAsync_WhenCancelledTripIsChanged_Throws()
        {
            const string id = "trip";
            _tripRepository.Setup(r => r.GetTripByIdAsync(id)).ReturnsAsync(new TripDB { Id = id, Status = "Cancelada" });
            await Assert.ThrowsAsync<ArgumentException>(() => _tripServiceAPI.UpdateTripAsync(id, new TripDTOAPI { Status = "Agendada" }));
        }

        [Fact]
        public async Task UpdateTripAsync_WhenTripDoesNotExist_ReturnsNull()
        {
            const string id = "507f1f77bcf86cd799439011";
            _tripRepository.Setup(r => r.GetTripByIdAsync(id)).ReturnsAsync((TripDB?)null);
            Assert.Null(await _tripServiceAPI.UpdateTripAsync(id, new TripDTOAPI { Status = "Cancelada" }));
            _tripRepository.Verify(r => r.UpdateTripAsync(It.IsAny<string>(), It.IsAny<TripDB>()), Times.Never);
        }

        [Fact]
        public async Task DeleteTripAsync_WhenTripExists_ReturnsTrue()
        {
            const string id = "507f1f77bcf86cd799439011";
            _tripRepository.Setup(r => r.DeleteTripAsync(id)).ReturnsAsync(true);
            Assert.True(await _tripServiceAPI.DeleteTripAsync(id));
        }

        [Fact]
        public async Task DeleteTripAsync_WhenTripDoesNotExist_ReturnsFalse()
        {
            const string id = "507f1f77bcf86cd799439011";
            _tripRepository.Setup(r => r.DeleteTripAsync(id)).ReturnsAsync(false);
            Assert.False(await _tripServiceAPI.DeleteTripAsync(id));
        }
    }
}