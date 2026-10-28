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
        private readonly IMapper _mapper;
        private readonly IMaintenanceRepositoryDB? _maintenanceRepository;

        public TripServiceAPI(ITripRepositoryDB tripRepository, ITripPassengerRepositoryDB tripPassengerRepository, IBusRepositoryDB busRepository, IDriverRepositoryDB driverRepository, IRouteRepositoryDB routeRepository, IPassengerRepositoryDB passengerRepository, IMapper mapper, IMaintenanceRepositoryDB? maintenanceRepository = null)
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
            if (trip.PlannedDurationMinutes <= 0 || trip.PlannedDurationMinutes > 1440)
                throw new ArgumentException("A duração planejada da viagem deve estar entre 1 e 1440 minutos.");
            await EnsureNoMaintenanceConflictAsync(trip.BusId, trip.DepartureTime, trip.PlannedDurationMinutes, null);
            var tripDB = _mapper.Map<TripDB>(trip);
            tripDB.Status = Scheduled;
            tripDB.StartedAt = null;
            tripDB.ArrivalTime = null;
            tripDB.Duration = 0;
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
            if (requestedStatus == Scheduled || requestedStatus == InProgress)
            {
                var busId = trip.BusId ?? existingTrip.BusId;
                var departure = trip.DepartureTime == default ? existingTrip.DepartureTime : trip.DepartureTime;
                var plannedDuration = trip.PlannedDurationMinutes > 0 ? trip.PlannedDurationMinutes : existingTrip.PlannedDurationMinutes;
                await EnsureNoMaintenanceConflictAsync(busId, departure, plannedDuration, id);
                existingTrip.BusId = busId;
                existingTrip.DepartureTime = departure;
                existingTrip.PlannedDurationMinutes = plannedDuration;
            }

            if (requestedStatus == InProgress)
            {
                if (DateTime.UtcNow < existingTrip.DepartureTime)
                    throw new ArgumentException("A viagem só pode entrar em andamento no horário previsto de saída ou depois dele.");
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

        private async Task EnsureNoMaintenanceConflictAsync(string? busId, DateTime departure, int durationMinutes, string? ignoredTripId)
        {
            if (_maintenanceRepository is null || string.IsNullOrWhiteSpace(busId))
                return;
            var tripStart = departure.Kind == DateTimeKind.Utc ? departure : departure.ToUniversalTime();
            var tripEnd = tripStart.AddMinutes(durationMinutes);
            var maintenanceItems = await _maintenanceRepository.GetAllAsync();
            var conflict = maintenanceItems.FirstOrDefault(item =>
                !item.IsDeleted &&
                string.Equals(item.BusId, busId, StringComparison.OrdinalIgnoreCase) &&
                item.Status is "Agendada" or "Em andamento" &&
                item.PlannedDurationMinutes > 0 &&
                IntervalsOverlap(
                    tripStart, tripEnd,
                    item.ServiceDate.Date.Add(item.ServiceStartTime.TimeOfDay),
                    item.ServiceDate.Date.Add(item.ServiceStartTime.TimeOfDay).AddMinutes(item.PlannedDurationMinutes)));
            if (conflict is not null)
                throw new ArgumentException($"O ônibus possui manutenção ativa no intervalo solicitado ({conflict.ServiceType}).");
        }

        private static bool IntervalsOverlap(DateTime firstStart, DateTime firstEnd, DateTime secondStart, DateTime secondEnd)
            => firstStart < secondEnd && secondStart < firstEnd;

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