using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Moq;

namespace BusTrack.Tests.UnitTests.ServicesAPI
{
    public class OperationalReportServiceAPIUnitTests
    {
        [Fact]
        public async Task GetReportAsync_DeveAgruparViagensOnibusMotoristaEManutencoesPorPeriodo()
        {
            var tripRepository = new Mock<ITripRepositoryDB>();
            var maintenanceRepository = new Mock<IMaintenanceRepositoryDB>();
            var busRepository = new Mock<IBusRepositoryDB>();
            var driverRepository = new Mock<IDriverRepositoryDB>();

            tripRepository.Setup(repository => repository.GetAllTripsAsync())
                .ReturnsAsync(new List<TripDB>
                {
                    new()
                    {
                        Id = "trip-1",
                        BusId = "bus-1",
                        DriverId = "driver-1",
                        DepartureTime = new DateTime(2026, 10, 10, 8, 0, 0, DateTimeKind.Utc),
                        Status = "Concluída"
                    },
                    new()
                    {
                        Id = "trip-2",
                        BusId = "bus-1",
                        DriverId = "driver-1",
                        DepartureTime = new DateTime(2026, 10, 12, 9, 0, 0, DateTimeKind.Utc),
                        Status = "Cancelada"
                    },
                    new()
                    {
                        Id = "trip-3",
                        BusId = "bus-2",
                        DriverId = "driver-2",
                        DepartureTime = new DateTime(2026, 11, 1, 9, 0, 0, DateTimeKind.Utc),
                        Status = "Concluída"
                    }
                });

            maintenanceRepository.Setup(repository => repository.GetAllAsync())
                .ReturnsAsync(new List<MaintenanceDB>
                {
                    new()
                    {
                        Id = "maintenance-1",
                        BusId = "bus-1",
                        ServiceDate = new DateTime(2026, 10, 11),
                        Cost = 150.50m,
                        Status = "Concluída"
                    },
                    new()
                    {
                        Id = "maintenance-2",
                        BusId = "bus-1",
                        ServiceDate = new DateTime(2026, 10, 20),
                        Cost = 50m,
                        Status = "Cancelada"
                    },
                    new()
                    {
                        Id = "maintenance-3",
                        BusId = "bus-2",
                        ServiceDate = new DateTime(2026, 11, 1),
                        Cost = 500m,
                        Status = "Concluída"
                    }
                });

            busRepository.Setup(repository => repository.GetAllBusesAsync())
                .ReturnsAsync(new List<BusDB>
                {
                    new() { Id = "bus-1", Number = "001", Plate = "ABC-1234" },
                    new() { Id = "bus-2", Number = "002", Plate = "DEF-5678" }
                });

            driverRepository.Setup(repository => repository.GetAllDriversAsync())
                .ReturnsAsync(new List<DriverDB>
                {
                    new() { Id = "driver-1", Name = "João" },
                    new() { Id = "driver-2", Name = "Maria" }
                });

            var service = new OperationalReportServiceAPI(
                tripRepository.Object,
                maintenanceRepository.Object,
                busRepository.Object,
                driverRepository.Object);

            var report = await service.GetReportAsync(
                new DateTime(2026, 10, 1),
                new DateTime(2026, 10, 31));

            Assert.Equal(2, report.TotalTrips);
            Assert.Equal(1, report.CompletedTrips);
            Assert.Equal(1, report.CancelledTrips);
            Assert.Equal(0, report.ScheduledTrips);
            Assert.Equal(0, report.InProgressTrips);
            Assert.Equal(200.50m, report.TotalMaintenanceCost);

            var bus = Assert.Single(report.BusUtilization);
            Assert.Equal("bus-1", bus.BusId);
            Assert.Equal("001", bus.BusNumber);
            Assert.Equal(2, bus.TripCount);
            Assert.Equal(1, bus.CompletedTripCount);

            var driver = Assert.Single(report.DriverTrips);
            Assert.Equal("driver-1", driver.DriverId);
            Assert.Equal("João", driver.DriverName);
            Assert.Equal(2, driver.TripCount);
            Assert.Equal(1, driver.CompletedTripCount);

            var maintenance = Assert.Single(report.MaintenanceCosts);
            Assert.Equal("bus-1", maintenance.BusId);
            Assert.Equal("001", maintenance.BusNumber);
            Assert.Equal(200.50m, maintenance.TotalCost);
            Assert.Equal(2, maintenance.MaintenanceCount);
        }

        [Fact]
        public async Task GetReportAsync_DeveRejeitarPeriodoInvalido()
        {
            var service = new OperationalReportServiceAPI(
                new Mock<ITripRepositoryDB>().Object,
                new Mock<IMaintenanceRepositoryDB>().Object,
                new Mock<IBusRepositoryDB>().Object,
                new Mock<IDriverRepositoryDB>().Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.GetReportAsync(
                    new DateTime(2026, 10, 31),
                    new DateTime(2026, 10, 1)));
        }
    }
}