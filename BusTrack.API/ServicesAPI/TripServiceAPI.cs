using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class TripServiceAPI : ITripServiceAPI
    {
        private const string Scheduled = "Agendada";
        private const string InProgress = "Em andamento";
        private const string Completed = "Concluída";
        private const string Cancelled = "Cancelada";
        private readonly ITripRepositoryDB _tripRepository;
        private readonly ITripPassengerRepositoryDB _tripPassengerRepository;
        private readonly IBusRepositoryDB _busRepository;
        private readonly IDriverRepositoryDB _driverRepository;
        private readonly IRouteRepositoryDB _routeRepository;
        private readonly IPassengerRepositoryDB _passengerRepository;
        private readonly IMaintenanceRepositoryDB? _maintenanceRepository;
        private readonly IMapper _mapper;

        public TripServiceAPI(
            ITripRepositoryDB tripRepository,
            ITripPassengerRepositoryDB tripPassengerRepository,
            IBusRepositoryDB busRepository,
            IDriverRepositoryDB driverRepository,
            IRouteRepositoryDB routeRepository,
            IPassengerRepositoryDB passengerRepository,
            IMapper mapper,
            IMaintenanceRepositoryDB? maintenanceRepository = null)
        {
            _tripRepository = tripRepository;
            _tripPassengerRepository = tripPassengerRepository;
            _busRepository = busRepository;
            _driverRepository = driverRepository;
            _routeRepository = routeRepository;
            _passengerRepository = passengerRepository;
            _mapper = mapper;
            _maintenanceRepository = maintenanceRepository;
        }

        public async Task<IEnumerable<TripDTOAPI>> GetAllTripsAsync()
        {
            var trips = await _tripRepository.GetAllTripsAsync();
            return _mapper.Map<IEnumerable<TripDTOAPI>>(trips);
        }

        public async Task<TripDTOAPI?> GetTripByIdAsync(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var trip = await _tripRepository.GetTripByIdAsync(id);
            return trip == null ? null : _mapper.Map<TripDTOAPI>(trip);
        }

        public async Task<IEnumerable<TripDetailsDTOAPI>> GetAllTripDetailsAsync()
        {
            var trips = await _tripRepository.GetAllTripsAsync();
            var result = new List<TripDetailsDTOAPI>();
            foreach (var trip in trips) result.Add(await BuildTripDetailsAsync(trip));
            return result;
        }

        public async Task<TripDetailsDTOAPI?> GetTripDetailsByIdAsync(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var trip = await _tripRepository.GetTripByIdAsync(id);
            return trip == null ? null : await BuildTripDetailsAsync(trip);
        }

        public async Task<TripDTOAPI> CreateTripAsync(TripDTOAPI trip)
        {
            if (trip.DepartureTime <= DateTime.UtcNow)
                throw new ArgumentException("A saída deve ser agendada para um horário futuro.");
            ValidatePlannedDuration(trip.PlannedDuration);
            await EnsureBusAvailableForTripAsync(trip.BusId, trip.DepartureTime);
            await ValidateResourceAvailabilityAsync(trip.BusId, trip.DriverId, trip.DepartureTime, trip.PlannedDuration, null);

            var tripDB = _mapper.Map<TripDB>(trip);
            tripDB.Status = Scheduled;
            tripDB.StartedAt = null;
            tripDB.ArrivalTime = null;
            tripDB.Duration = 0;
            tripDB.PlannedDuration = trip.PlannedDuration;
            tripDB.LimitPassengers = 0;
            tripDB.Passengers = new List<string>();
            var createdTrip = await _tripRepository.AddTripAsync(tripDB);
            return _mapper.Map<TripDTOAPI>(createdTrip);
        }

        public async Task<TripDTOAPI?> UpdateTripAsync(string? id, TripDTOAPI trip)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var existingTrip = await _tripRepository.GetTripByIdAsync(id);
            if (existingTrip == null) return null;

            var currentStatus = NormalizeStatus(existingTrip.Status);
            var requestedStatus = NormalizeStatus(trip.Status);
            if (currentStatus == Completed || currentStatus == Cancelled)
                throw new ArgumentException("Viagens concluídas ou canceladas não podem ser alteradas.");
            if (!IsAllowedTransition(currentStatus, requestedStatus))
                throw new ArgumentException($"Transição de '{currentStatus}' para '{requestedStatus}' não permitida.");

            if (trip.PlannedDuration > 0)
                existingTrip.PlannedDuration = trip.PlannedDuration;

            if (requestedStatus == InProgress)
            {
                ValidatePlannedDuration(existingTrip.PlannedDuration);
                if (DateTime.UtcNow < existingTrip.DepartureTime)
                    throw new ArgumentException("A viagem só pode entrar em andamento no horário previsto de saída ou depois dele.");
                await ValidateResourceAvailabilityAsync(existingTrip.BusId, existingTrip.DriverId, existingTrip.DepartureTime, existingTrip.PlannedDuration, existingTrip.Id);
                var now = DateTime.UtcNow;
                existingTrip.Status = InProgress;
                existingTrip.StartedAt = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, now.Kind);
                existingTrip.ArrivalTime = null;
                existingTrip.Duration = 0;
                existingTrip.LimitPassengers = 0;
            }
            else if (requestedStatus == Cancelled)
            {
                existingTrip.Status = Cancelled;
                existingTrip.ArrivalTime = null;
                existingTrip.Duration = 0;
                existingTrip.LimitPassengers = 0;
                existingTrip.Passengers = new List<string>();
            }
            else if (requestedStatus == Completed)
            {
                var startedAt = existingTrip.StartedAt;
                if (currentStatus == Scheduled)
                {
                    if (DateTime.UtcNow < existingTrip.DepartureTime)
                        throw new ArgumentException("A viagem não pode ser concluída antes do horário previsto de saída.");
                    startedAt = existingTrip.DepartureTime;
                }
                if (startedAt == null)
                    throw new ArgumentException("Registre o início da viagem antes de concluí-la.");
                if (trip.ArrivalTime == null || trip.ArrivalTime == default)
                    throw new ArgumentException("Informe a data e o horário reais de chegada para concluir a viagem.");
                if (trip.ArrivalTime.Value > DateTime.UtcNow)
                    throw new ArgumentException("A chegada real não pode estar no futuro.");
                if (trip.ArrivalTime.Value < startedAt.Value)
                    throw new ArgumentException("A chegada não pode ser anterior ao início da viagem.");
                if (trip.LimitPassengers <= 0)
                    throw new ArgumentException("Informe a quantidade de passageiros transportados para concluir a viagem.");

                var calculatedDuration = (int)Math.Floor((trip.ArrivalTime.Value - startedAt.Value).TotalMinutes);
                if (calculatedDuration <= 0)
                    throw new ArgumentException("A duração da viagem concluída deve ser maior que zero minuto.");
                if (trip.Duration != calculatedDuration)
                    throw new ArgumentException($"A duração informada deve corresponder ao tempo real da viagem: {calculatedDuration} minuto(s).");

                existingTrip.Status = Completed;
                existingTrip.StartedAt = startedAt;
                existingTrip.ArrivalTime = trip.ArrivalTime.Value;
                existingTrip.Duration = calculatedDuration;
                existingTrip.LimitPassengers = trip.LimitPassengers;
                existingTrip.Passengers = trip.Passengers ?? new List<string>();
            }

            existingTrip.Id = id;
            var updatedTrip = await _tripRepository.UpdateTripAsync(id, existingTrip);
            return updatedTrip == null ? null : _mapper.Map<TripDTOAPI>(updatedTrip);
        }

        public async Task<bool> DeleteTripAsync(string? id)
        {
            if (string.IsNullOrWhiteSpace(id)) return false;
            return await _tripRepository.DeleteTripAsync(id);
        }

        private async Task ValidateResourceAvailabilityAsync(string? busId, string? driverId, DateTime departureTime, int plannedDuration, string? ignoredTripId)
        {
            if (string.IsNullOrWhiteSpace(busId) || string.IsNullOrWhiteSpace(driverId))
                throw new ArgumentException("Informe o ônibus e o motorista da viagem.");

            var departureEnd = departureTime.AddMinutes(plannedDuration);
            var trips = await _tripRepository.GetAllTripsAsync();

            foreach (var existing in trips)
            {
                if (!string.IsNullOrWhiteSpace(ignoredTripId) && existing.Id == ignoredTripId)
                    continue;
                if (NormalizeStatus(existing.Status) is Completed or Cancelled)
                    continue;
                if (existing.PlannedDuration <= 0)
                    continue;

                var existingEnd = existing.DepartureTime.AddMinutes(existing.PlannedDuration);
                var overlaps = departureTime < existingEnd && existing.DepartureTime < departureEnd;
                if (!overlaps)
                    continue;

                if (string.Equals(existing.BusId, busId, StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("O ônibus selecionado já está alocado em outra viagem no mesmo período.");
                if (string.Equals(existing.DriverId, driverId, StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("O motorista selecionado já está alocado em outra viagem no mesmo período.");
            }
        }

        private static void ValidatePlannedDuration(int plannedDuration)
        {
            if (plannedDuration <= 0)
                throw new ArgumentException("Informe a duração prevista da viagem em minutos.");
        }

        private async Task EnsureBusAvailableForTripAsync(
            string? busId,
            DateTime departureTime)
        {
            if (string.IsNullOrWhiteSpace(busId) || _maintenanceRepository is null)
                return;

            var maintenanceItems = await _maintenanceRepository.GetAllAsync();

            var tripDate = GetBrazilDate(departureTime);

            var blocksBus = maintenanceItems.Any(maintenance =>
                !maintenance.IsDeleted &&
                string.Equals(maintenance.BusId, busId, StringComparison.OrdinalIgnoreCase) &&
                IsActiveMaintenanceStatus(maintenance.Status) &&
                GetBrazilDate(maintenance.ServiceDate) == tripDate);

            if (blocksBus)
            {
                throw new ArgumentException(
                    "O ônibus não está disponível para esta viagem porque possui manutenção agendada ou em andamento na data da viagem.");
            }
        }

        private static bool IsActiveMaintenanceStatus(string? status)
        {
            return string.Equals(status?.Trim(), "Agendada", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(status?.Trim(), "Em andamento", StringComparison.OrdinalIgnoreCase);
        }

        private static DateTime GetBrazilDate(DateTime value)
        {
            var utc = value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };

            TimeZoneInfo timeZone;
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
            }
            catch (TimeZoneNotFoundException)
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            }

            return TimeZoneInfo.ConvertTimeFromUtc(utc, timeZone).Date;
        }

        private static string NormalizeStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status)) return Scheduled;
            return status.Trim().ToLowerInvariant() switch
            {
                "agendada" => Scheduled,
                "em andamento" => InProgress,
                "concluída" or "concluida" => Completed,
                "cancelada" => Cancelled,
                _ => throw new ArgumentException("Status de viagem inválido.")
            };
        }

        private static bool IsAllowedTransition(string current, string requested) => current switch
        {
            Scheduled => requested is InProgress or Completed or Cancelled,
            InProgress => requested is Completed or Cancelled,
            _ => false
        };

        private async Task<TripDetailsDTOAPI> BuildTripDetailsAsync(TripDB trip)
        {
            var details = new TripDetailsDTOAPI
            {
                Id = trip.Id,
                UserId = trip.UserId,
                DepartureTime = trip.DepartureTime,
                StartedAt = trip.StartedAt,
                ArrivalTime = trip.ArrivalTime,
                Duration = trip.Duration,
                PlannedDuration = trip.PlannedDuration,
                LimitPassengers = trip.LimitPassengers,
                Status = string.IsNullOrWhiteSpace(trip.Status) ? Scheduled : trip.Status
            };

            if (!string.IsNullOrWhiteSpace(trip.BusId))
            {
                var bus = await _busRepository.GetBusByIdAsync(trip.BusId);
                if (bus != null) details.Bus = new BusDetailsDTOAPI { Id = bus.Id, Number = bus.Number, LicensePlate = bus.Plate, Model = bus.Line, Capacity = bus.Capacity };
            }
            if (!string.IsNullOrWhiteSpace(trip.DriverId))
            {
                var driver = await _driverRepository.GetDriverByIdAsync(trip.DriverId);
                if (driver != null) details.Driver = new DriverDetailsDTOAPI { Id = driver.Id, Name = driver.Name, LicenseNumber = driver.Cpf };
            }
            if (!string.IsNullOrWhiteSpace(trip.RouteId))
            {
                var route = await _routeRepository.GetRouteByIdAsync(trip.RouteId);
                if (route != null) details.Route = new RouteDetailsDTOAPI { Id = route.Id, Name = route.Name, Origin = route.Origin, Destination = route.Destination };
            }

            var tripPassengers = await _tripPassengerRepository.GetByTripIdAsync(trip.Id ?? string.Empty);
            var passengerIds = tripPassengers.Select(item => item.PassengerId).Where(passengerId => !string.IsNullOrWhiteSpace(passengerId)).Select(passengerId => passengerId!).Distinct().ToList();
            if (passengerIds.Count == 0 && trip.Passengers != null)
                passengerIds = trip.Passengers.Where(passengerId => !string.IsNullOrWhiteSpace(passengerId)).Select(passengerId => passengerId!).Distinct().ToList();

            foreach (var passengerId in passengerIds)
            {
                var passenger = await _passengerRepository.GetPassengerByIdAsync(passengerId);
                if (passenger == null) continue;
                details.Passengers.Add(new PassengerDetailsDTOAPI { Id = passenger.Id, Name = passenger.Name, Cpf = passenger.Cpf, Email = passenger.Email, Phone = passenger.Phone });
            }
            return details;
        }
    }
}