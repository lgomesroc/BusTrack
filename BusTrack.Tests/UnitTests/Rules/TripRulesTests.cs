
using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.Tests.MappingsIntegrationTests;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BusTrack.Tests.UnitTests.Rules
{
    public class TripRulesTests
    {
        private readonly Mock<ITripRepositoryDB> _tripRepository = new();
        private readonly Mock<ITripPassengerRepositoryDB> _tripPassengerRepository = new();
        private readonly Mock<IBusRepositoryDB> _busRepository = new();
        private readonly Mock<IDriverRepositoryDB> _driverRepository = new();
        private readonly Mock<IRouteRepositoryDB> _routeRepository = new();
        private readonly Mock<IPassengerRepositoryDB> _passengerRepository = new();
        private readonly Mock<IMaintenanceRepositoryDB> _maintenanceRepository = new();

        private readonly TripServiceAPI _service;

        public TripRulesTests()
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
                configuration.CreateMapper(),
                _maintenanceRepository.Object);

            _tripRepository
                .Setup(repository => repository.GetAllTripsAsync())
                .ReturnsAsync(new List<TripDB>());

            _maintenanceRepository
                .Setup(repository => repository.GetAllAsync())
                .ReturnsAsync(new List<MaintenanceDB>());
        }

        [Fact]
        public async Task CreateTrip_WhenDepartureIsInThePast_Rejects()
        {
            var trip = CreateTrip(DateTime.UtcNow.AddMinutes(-1));

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateTripAsync(trip));

            Assert.Equal(
                "A saída deve ser agendada para um horário futuro.",
                exception.Message);

            _tripRepository.Verify(
                repository => repository.AddTripAsync(It.IsAny<TripDB>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateTrip_WhenDepartureIsFuture_StartsAsScheduled()
        {
            var trip = CreateTrip(DateTime.UtcNow.AddMinutes(10));

            _tripRepository
                .Setup(repository => repository.AddTripAsync(It.IsAny<TripDB>()))
                .ReturnsAsync((TripDB value) => value);

            var result = await _service.CreateTripAsync(trip);

            Assert.Equal("Agendada", result.Status);
            Assert.Null(result.StartedAt);
            Assert.Null(result.ArrivalTime);
            Assert.Equal(0, result.Duration);
            Assert.Equal(0, result.LimitPassengers);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(1441)]
        public async Task CreateTrip_WhenPlannedDurationIsInvalid_Rejects(
            int plannedDurationMinutes)
        {
            var trip = CreateTrip(DateTime.UtcNow.AddMinutes(10));
            trip.PlannedDuration = plannedDurationMinutes;

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateTripAsync(trip));

            Assert.Contains("duração planejada", exception.Message);
            _tripRepository.Verify(
                repository => repository.AddTripAsync(It.IsAny<TripDB>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateTrip_WhenMaintenanceOverlaps_Rejects()
        {
            var departure = DateTime.UtcNow.AddHours(2);
            var trip = CreateTrip(departure);

            var timeZone = GetSaoPauloTimeZone();
            var localMaintenanceStart = TimeZoneInfo.ConvertTimeFromUtc(
                departure.AddMinutes(-15),
                timeZone);

            var maintenance = new MaintenanceDB
            {
                Id = "maintenance",
                BusId = trip.BusId!,
                ServiceType = "Revisão",
                ServiceDate = localMaintenanceStart.Date,
                Status = "Agendada",
                IsDeleted = false
            };

            _maintenanceRepository
                .Setup(repository => repository.GetAllAsync())
                .ReturnsAsync(new List<MaintenanceDB> { maintenance });

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateTripAsync(trip));

            Assert.Contains("manutenção", exception.Message.ToLowerInvariant());

            _tripRepository.Verify(
                repository => repository.AddTripAsync(It.IsAny<TripDB>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateTrip_WhenMaintenanceIsDeleted_DoesNotRejectForThatMaintenance()
        {
            var departure = DateTime.UtcNow.AddHours(2);
            var trip = CreateTrip(departure);

            var timeZone = GetSaoPauloTimeZone();
            var localMaintenanceStart = TimeZoneInfo.ConvertTimeFromUtc(
                departure.AddMinutes(-15),
                timeZone);

            var maintenance = new MaintenanceDB
            {
                Id = "maintenance",
                BusId = trip.BusId!,
                ServiceType = "Revisão",
                ServiceDate = localMaintenanceStart.Date,
                Status = "Agendada",
                IsDeleted = true
            };

            _maintenanceRepository
                .Setup(repository => repository.GetAllAsync())
                .ReturnsAsync(new List<MaintenanceDB> { maintenance });

            _tripRepository
                .Setup(repository => repository.AddTripAsync(It.IsAny<TripDB>()))
                .ReturnsAsync((TripDB value) => value);

            var result = await _service.CreateTripAsync(trip);

            Assert.Equal("Agendada", result.Status);
        }

        [Fact]
        public async Task ScheduledTrip_CanMoveToInProgressAtDepartureTime()
        {
            var departure = DateTime.UtcNow.AddMinutes(-1);
            var existing = CreateDbTrip("Agendada", departure);

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            _tripRepository
                .Setup(repository => repository.UpdateTripAsync(
                    existing.Id!,
                    It.IsAny<TripDB>()))
                .ReturnsAsync((string _, TripDB value) => value);

            var result = await _service.UpdateTripAsync(
                existing.Id,
                new TripDTOAPI { Status = "Em andamento" });

            Assert.Equal("Em andamento", result!.Status);
            Assert.NotNull(result.StartedAt);
            Assert.Equal(0, result.Duration);
            Assert.Equal(0, result.LimitPassengers);
        }

        [Fact]
        public async Task ScheduledTrip_CannotMoveToInProgressBeforeDepartureTime()
        {
            var existing = CreateDbTrip(
                "Agendada",
                DateTime.UtcNow.AddMinutes(5));

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateTripAsync(
                    existing.Id,
                    new TripDTOAPI { Status = "Em andamento" }));

            Assert.Contains("só pode entrar em andamento", exception.Message);

            _tripRepository.Verify(
                repository => repository.UpdateTripAsync(
                    It.IsAny<string>(),
                    It.IsAny<TripDB>()),
                Times.Never);
        }

        [Fact]
        public async Task ScheduledTrip_CanBeCancelled()
        {
            var existing = CreateDbTrip(
                "Agendada",
                DateTime.UtcNow.AddMinutes(10));

            existing.ArrivalTime = DateTime.UtcNow.AddMinutes(20);
            existing.Duration = 20;
            existing.LimitPassengers = 30;

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            _tripRepository
                .Setup(repository => repository.UpdateTripAsync(
                    existing.Id!,
                    It.IsAny<TripDB>()))
                .ReturnsAsync((string _, TripDB value) => value);

            var result = await _service.UpdateTripAsync(
                existing.Id,
                new TripDTOAPI { Status = "Cancelada" });

            Assert.Equal("Cancelada", result!.Status);
            Assert.Null(result.ArrivalTime);
            Assert.Equal(0, result.Duration);
            Assert.Equal(0, result.LimitPassengers);
            Assert.Empty(result.Passengers!);
        }

        [Fact]
        public async Task InProgressTrip_CanBeCompleted()
        {
            var now = DateTime.UtcNow;
            var started = new DateTime(
                now.Year,
                now.Month,
                now.Day,
                now.Hour,
                now.Minute,
                0,
                DateTimeKind.Utc).AddMinutes(-59);

            var arrival = started.AddMinutes(59);
            var existing = CreateDbTrip("Em andamento", started);
            existing.StartedAt = started;

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            _tripRepository
                .Setup(repository => repository.UpdateTripAsync(
                    existing.Id!,
                    It.IsAny<TripDB>()))
                .ReturnsAsync((string _, TripDB value) => value);

            var result = await _service.UpdateTripAsync(
                existing.Id,
                new TripDTOAPI
                {
                    Status = "Concluída",
                    ArrivalTime = arrival,
                    Duration = 59,
                    LimitPassengers = 35
                });

            Assert.Equal("Concluída", result!.Status);
            Assert.Equal(arrival, result.ArrivalTime);
            Assert.Equal(59, result.Duration);
            Assert.Equal(35, result.LimitPassengers);
        }

        [Fact]
        public async Task Completion_RequiresArrivalTime()
        {
            var started = DateTime.UtcNow.AddMinutes(-30);
            var existing = CreateDbTrip("Em andamento", started);
            existing.StartedAt = started;

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateTripAsync(
                    existing.Id,
                    new TripDTOAPI
                    {
                        Status = "Concluída",
                        Duration = 30,
                        LimitPassengers = 20
                    }));

            Assert.Contains("data e o horário reais de chegada", exception.Message);
        }

        [Fact]
        public async Task Completion_RejectsFutureArrival()
        {
            var started = DateTime.UtcNow.AddMinutes(-10);
            var existing = CreateDbTrip("Em andamento", started);
            existing.StartedAt = started;

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateTripAsync(
                    existing.Id,
                    new TripDTOAPI
                    {
                        Status = "Concluída",
                        ArrivalTime = DateTime.UtcNow.AddMinutes(1),
                        Duration = 11,
                        LimitPassengers = 20
                    }));

            Assert.Contains("não pode estar no futuro", exception.Message);
        }

        [Fact]
        public async Task Completion_RejectsArrivalBeforeStart()
        {
            var started = DateTime.UtcNow.AddMinutes(-10);
            var existing = CreateDbTrip("Em andamento", started);
            existing.StartedAt = started;

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateTripAsync(
                    existing.Id,
                    new TripDTOAPI
                    {
                        Status = "Concluída",
                        ArrivalTime = started.AddMinutes(-1),
                        Duration = 0,
                        LimitPassengers = 20
                    }));

            Assert.Contains("anterior ao início", exception.Message);
        }

        [Fact]
        public async Task Completion_RequiresPositivePassengerCount()
        {
            var started = DateTime.UtcNow.AddMinutes(-30);
            var arrival = started.AddMinutes(30);
            var existing = CreateDbTrip("Em andamento", started);
            existing.StartedAt = started;

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateTripAsync(
                    existing.Id,
                    new TripDTOAPI
                    {
                        Status = "Concluída",
                        ArrivalTime = arrival,
                        Duration = 30,
                        LimitPassengers = 0
                    }));

            Assert.Contains("quantidade de passageiros", exception.Message);
        }

        [Fact]
        public async Task Completion_RequiresExactDuration()
        {
            var started = DateTime.UtcNow.AddMinutes(-30);
            var arrival = started.AddMinutes(30);
            var existing = CreateDbTrip("Em andamento", started);
            existing.StartedAt = started;

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateTripAsync(
                    existing.Id,
                    new TripDTOAPI
                    {
                        Status = "Concluída",
                        ArrivalTime = arrival,
                        Duration = 29,
                        LimitPassengers = 20
                    }));

            Assert.Contains("duração informada deve corresponder", exception.Message);
        }

        [Fact]
        public async Task ScheduledTrip_CanMoveDirectlyToCompletedWithValidData()
        {
            var departure = DateTime.UtcNow.AddMinutes(-30);
            var arrival = departure.AddMinutes(30);
            var existing = CreateDbTrip("Agendada", departure);

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            _tripRepository
                .Setup(repository => repository.UpdateTripAsync(
                    existing.Id!,
                    It.IsAny<TripDB>()))
                .ReturnsAsync((string _, TripDB value) => value);

            var result = await _service.UpdateTripAsync(
                existing.Id,
                new TripDTOAPI
                {
                    Status = "Concluída",
                    ArrivalTime = arrival,
                    Duration = 30,
                    LimitPassengers = 25
                });

            Assert.Equal("Concluída", result!.Status);
            Assert.Equal(arrival, result.ArrivalTime);
            Assert.Equal(30, result.Duration);
            Assert.Equal(25, result.LimitPassengers);
        }

        [Fact]
        public async Task ScheduledTrip_CannotRemainScheduledThroughAnUpdate()
        {
            var existing = CreateDbTrip(
                "Agendada",
                DateTime.UtcNow.AddMinutes(10));

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateTripAsync(
                    existing.Id,
                    new TripDTOAPI { Status = "Agendada" }));

            Assert.Contains(
                "Transição de 'Agendada' para 'Agendada' não permitida",
                exception.Message);
        }

        [Fact]
        public async Task InProgressTrip_CannotReturnToScheduled()
        {
            var started = DateTime.UtcNow.AddMinutes(-5);
            var existing = CreateDbTrip("Em andamento", started);
            existing.StartedAt = started;

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateTripAsync(
                    existing.Id,
                    new TripDTOAPI { Status = "Agendada" }));

            Assert.Contains(
                "Transição de 'Em andamento' para 'Agendada' não permitida",
                exception.Message);
        }

        [Fact]
        public async Task CompletedTrip_CannotChangeStatus()
        {
            var existing = CreateDbTrip(
                "Concluída",
                DateTime.UtcNow.AddMinutes(-60));

            existing.StartedAt = DateTime.UtcNow.AddMinutes(-60);
            existing.ArrivalTime = DateTime.UtcNow.AddMinutes(-1);
            existing.Duration = 59;
            existing.LimitPassengers = 30;

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateTripAsync(
                    existing.Id,
                    new TripDTOAPI { Status = "Cancelada" }));

            _tripRepository.Verify(
                repository => repository.UpdateTripAsync(
                    It.IsAny<string>(),
                    It.IsAny<TripDB>()),
                Times.Never);
        }

        [Fact]
        public async Task CancelledTrip_CannotChangeStatus()
        {
            var existing = CreateDbTrip(
                "Cancelada",
                DateTime.UtcNow.AddMinutes(-60));

            _tripRepository
                .Setup(repository => repository.GetTripByIdAsync(existing.Id!))
                .ReturnsAsync(existing);

            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.UpdateTripAsync(
                    existing.Id,
                    new TripDTOAPI { Status = "Agendada" }));

            _tripRepository.Verify(
                repository => repository.UpdateTripAsync(
                    It.IsAny<string>(),
                    It.IsAny<TripDB>()),
                Times.Never);
        }

        private static TripDTOAPI CreateTrip(DateTime departure) => new()
        {
            BusId = "bus",
            DriverId = "driver",
            RouteId = "route",
            DepartureTime = departure,
            PlannedDuration = 60,
            Status = "Agendada"
        };

        private static TripDB CreateDbTrip(
            string status,
            DateTime departure) => new()
        {
            Id = "trip",
            BusId = "bus",
            DriverId = "driver",
            RouteId = "route",
            DepartureTime = departure,
            PlannedDuration = 60,
            Status = status,
            Duration = 0,
            LimitPassengers = 0,
            Passengers = new List<string>()
        };

        private static TimeZoneInfo GetSaoPauloTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById(
                    "E. South America Standard Time");
            }
        }
    }
}
