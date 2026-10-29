using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class OperationalReportServiceAPI : IOperationalReportServiceAPI
    {
        private readonly ITripRepositoryDB _tripRepository;
        private readonly IMaintenanceRepositoryDB _maintenanceRepository;
        private readonly IBusRepositoryDB _busRepository;
        private readonly IDriverRepositoryDB _driverRepository;

        public OperationalReportServiceAPI(
            ITripRepositoryDB tripRepository,
            IMaintenanceRepositoryDB maintenanceRepository,
            IBusRepositoryDB busRepository,
            IDriverRepositoryDB driverRepository)
        {
            _tripRepository = tripRepository;
            _maintenanceRepository = maintenanceRepository;
            _busRepository = busRepository;
            _driverRepository = driverRepository;
        }

        public async Task<OperationalReportDTOAPI> GetReportAsync(
            DateTime startDate,
            DateTime endDate)
        {
            var start = startDate.Date;
            var endExclusive = endDate.Date.AddDays(1);

            if (endExclusive <= start)
            {
                throw new ArgumentException("A data final deve ser igual ou posterior à data inicial.");
            }

            var trips = (await _tripRepository.GetAllTripsAsync())
                .Where(trip => trip.DepartureTime >= start && trip.DepartureTime < endExclusive)
                .ToList();

            var maintenances = (await _maintenanceRepository.GetAllAsync())
                .Where(maintenance => maintenance.ServiceDate >= start && maintenance.ServiceDate < endExclusive)
                .ToList();

            var buses = (await _busRepository.GetAllBusesAsync()).ToDictionary(
                bus => bus.Id ?? string.Empty,
                StringComparer.OrdinalIgnoreCase);

            var drivers = (await _driverRepository.GetAllDriversAsync()).ToDictionary(
                driver => driver.Id ?? string.Empty,
                StringComparer.OrdinalIgnoreCase);

            var report = new OperationalReportDTOAPI
            {
                StartDate = start,
                EndDate = endDate.Date,
                TotalTrips = trips.Count,
                CompletedTrips = trips.Count(trip => IsStatus(trip.Status, "Concluída", "Concluida")),
                CancelledTrips = trips.Count(trip => IsStatus(trip.Status, "Cancelada")),
                ScheduledTrips = trips.Count(trip => IsStatus(trip.Status, "Agendada")),
                InProgressTrips = trips.Count(trip => IsStatus(trip.Status, "Em andamento")),
                TotalMaintenanceCost = maintenances.Sum(maintenance => maintenance.Cost)
            };

            report.BusUtilization = trips
                .Where(trip => !string.IsNullOrWhiteSpace(trip.BusId))
                .GroupBy(trip => trip.BusId!, StringComparer.OrdinalIgnoreCase)
                .Select(group => new BusUtilizationReportDTOAPI
                {
                    BusId = group.Key,
                    BusNumber = GetBusNumber(buses, group.Key),
                    TripCount = group.Count(),
                    CompletedTripCount = group.Count(trip => IsStatus(trip.Status, "Concluída", "Concluida"))
                })
                .OrderByDescending(item => item.TripCount)
                .ThenBy(item => item.BusNumber)
                .ToList();

            report.DriverTrips = trips
                .Where(trip => !string.IsNullOrWhiteSpace(trip.DriverId))
                .GroupBy(trip => trip.DriverId!, StringComparer.OrdinalIgnoreCase)
                .Select(group => new DriverTripReportDTOAPI
                {
                    DriverId = group.Key,
                    DriverName = GetDriverName(drivers, group.Key),
                    TripCount = group.Count(),
                    CompletedTripCount = group.Count(trip => IsStatus(trip.Status, "Concluída", "Concluida"))
                })
                .OrderByDescending(item => item.TripCount)
                .ThenBy(item => item.DriverName)
                .ToList();

            report.MaintenanceCosts = maintenances
                .Where(maintenance => !string.IsNullOrWhiteSpace(maintenance.BusId))
                .GroupBy(maintenance => maintenance.BusId, StringComparer.OrdinalIgnoreCase)
                .Select(group => new MaintenanceCostReportDTOAPI
                {
                    BusId = group.Key,
                    BusNumber = GetBusNumber(buses, group.Key),
                    TotalCost = group.Sum(maintenance => maintenance.Cost),
                    MaintenanceCount = group.Count()
                })
                .OrderByDescending(item => item.TotalCost)
                .ThenBy(item => item.BusNumber)
                .ToList();

            return report;
        }

        private static bool IsStatus(string? status, params string[] expected)
        {
            return expected.Any(item => string.Equals(
                status?.Trim(),
                item,
                StringComparison.OrdinalIgnoreCase));
        }

        private static string GetBusNumber(
            IReadOnlyDictionary<string, global::BusTrack.BusTrack.DB.Classes.BusDB> buses,
            string busId)
        {
            return buses.TryGetValue(busId, out var bus)
                ? !string.IsNullOrWhiteSpace(bus.Number)
                    ? bus.Number
                    : bus.Plate ?? busId
                : busId;
        }

        private static string GetDriverName(
            IReadOnlyDictionary<string, global::BusTrack.BusTrack.DB.Classes.DriverDB> drivers,
            string driverId)
        {
            return drivers.TryGetValue(driverId, out var driver)
                ? !string.IsNullOrWhiteSpace(driver.Name)
                    ? driver.Name
                    : driverId
                : driverId;
        }
    }
}