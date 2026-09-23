using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class TripServiceAPI : ITripServiceAPI
    {
        private readonly ITripRepositoryDB _tripRepository;

        private readonly ITripPassengerRepositoryDB
            _tripPassengerRepository;

        private readonly IBusRepositoryDB _busRepository;

        private readonly IDriverRepositoryDB _driverRepository;

        private readonly IRouteRepositoryDB _routeRepository;

        private readonly IPassengerRepositoryDB
            _passengerRepository;

        private readonly IMapper _mapper;

        public TripServiceAPI(
            ITripRepositoryDB tripRepository,
            ITripPassengerRepositoryDB tripPassengerRepository,
            IBusRepositoryDB busRepository,
            IDriverRepositoryDB driverRepository,
            IRouteRepositoryDB routeRepository,
            IPassengerRepositoryDB passengerRepository,
            IMapper mapper)
        {
            _tripRepository =
                tripRepository;

            _tripPassengerRepository =
                tripPassengerRepository;

            _busRepository =
                busRepository;

            _driverRepository =
                driverRepository;

            _routeRepository =
                routeRepository;

            _passengerRepository =
                passengerRepository;

            _mapper =
                mapper;
        }

        public async Task<IEnumerable<TripDTOAPI>>
            GetAllTripsAsync()
        {
            var trips =
                await _tripRepository
                    .GetAllTripsAsync();

            return _mapper.Map<
                IEnumerable<TripDTOAPI>>(
                trips);
        }

        public async Task<TripDTOAPI?>
            GetTripByIdAsync(
                string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var trip =
                await _tripRepository
                    .GetTripByIdAsync(id);

            if (trip == null)
            {
                return null;
            }

            return _mapper.Map<TripDTOAPI>(
                trip);
        }

        public async Task<IEnumerable<TripDetailsDTOAPI>>
            GetAllTripDetailsAsync()
        {
            var trips =
                await _tripRepository
                    .GetAllTripsAsync();

            var result =
                new List<TripDetailsDTOAPI>();

            foreach (var trip in trips)
            {
                var details =
                    await BuildTripDetailsAsync(
                        trip);

                result.Add(details);
            }

            return result;
        }

        public async Task<TripDetailsDTOAPI?>
            GetTripDetailsByIdAsync(
                string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var trip =
                await _tripRepository
                    .GetTripByIdAsync(id);

            if (trip == null)
            {
                return null;
            }

            return await BuildTripDetailsAsync(
                trip);
        }

        public async Task<TripDTOAPI>
            CreateTripAsync(
                TripDTOAPI trip)
        {
            var tripDB =
                _mapper.Map<TripDB>(
                    trip);

            var createdTrip =
                await _tripRepository
                    .AddTripAsync(
                        tripDB);

            return _mapper.Map<TripDTOAPI>(
                createdTrip);
        }

        public async Task<TripDTOAPI?>
            UpdateTripAsync(
                string? id,
                TripDTOAPI trip)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var existingTrip =
                await _tripRepository
                    .GetTripByIdAsync(id);

            if (existingTrip == null)
            {
                return null;
            }

            _mapper.Map(
                trip,
                existingTrip);

            existingTrip.Id = id;

            var updatedTrip =
                await _tripRepository
                    .UpdateTripAsync(
                        id,
                        existingTrip);

            if (updatedTrip == null)
            {
                return null;
            }

            return _mapper.Map<TripDTOAPI>(
                updatedTrip);
        }

        public async Task<bool>
            DeleteTripAsync(
                string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            return await _tripRepository
                .DeleteTripAsync(id);
        }

        private async Task<TripDetailsDTOAPI>
            BuildTripDetailsAsync(
                TripDB trip)
        {
            var details =
                new TripDetailsDTOAPI
                {
                    Id = trip.Id,

                    DepartureTime =
                        trip.DepartureTime,

                    ArrivalTime =
                        trip.ArrivalTime,

                    Duration =
                        trip.Duration,

                    LimitPassengers =
                        trip.LimitPassengers
                };

            if (!string.IsNullOrWhiteSpace(
                trip.BusId))
            {
                var bus =
                    await _busRepository
                        .GetBusByIdAsync(
                            trip.BusId);

                if (bus != null)
                {
                    details.Bus =
                        new BusDetailsDTOAPI
                        {
                            Id = bus.Id,

                            Number =
                                bus.Number,

                            LicensePlate =
                                bus.Plate,

                            Model =
                                bus.Line,

                            Capacity =
                                bus.Capacity
                        };
                }
            }

            if (!string.IsNullOrWhiteSpace(
                trip.DriverId))
            {
                var driver =
                    await _driverRepository
                        .GetDriverByIdAsync(
                            trip.DriverId);

                if (driver != null)
                {
                    details.Driver =
                        new DriverDetailsDTOAPI
                        {
                            Id = driver.Id,

                            Name =
                                driver.Name,

                            LicenseNumber =
                                driver.Cpf
                        };
                }
            }

            if (!string.IsNullOrWhiteSpace(
                trip.RouteId))
            {
                var route =
                    await _routeRepository
                        .GetRouteByIdAsync(
                            trip.RouteId);

                if (route != null)
                {
                    details.Route =
                        new RouteDetailsDTOAPI
                        {
                            Id = route.Id,

                            Name =
                                route.Name,

                            Origin =
                                route.Origin,

                            Destination =
                                route.Destination
                        };
                }
            }

            var tripPassengers =
                await _tripPassengerRepository
                    .GetByTripIdAsync(
                        trip.Id ?? string.Empty);

            var passengerIds =
                tripPassengers
                    .Select(
                        item => item.PassengerId)
                    .Where(
                        id =>
                            !string.IsNullOrWhiteSpace(
                                id))
                    .Select(
                        id => id!)
                    .Distinct()
                    .ToList();

            if (passengerIds.Count == 0 &&
                trip.Passengers != null)
            {
                passengerIds =
                    trip.Passengers
                        .Where(
                            id =>
                                !string.IsNullOrWhiteSpace(
                                    id))
                        .Select(
                            id => id!)
                        .Distinct()
                        .ToList();
            }

            foreach (var passengerId
                in passengerIds)
            {
                var passenger =
                    await _passengerRepository
                        .GetPassengerByIdAsync(
                            passengerId!);

                if (passenger == null)
                {
                    continue;
                }

                details.Passengers.Add(
                    new PassengerDetailsDTOAPI
                    {
                        Id =
                            passenger.Id,

                        Name =
                            passenger.Name,

                        Cpf =
                            passenger.Cpf,

                        Email =
                            passenger.Email,

                        Phone =
                            passenger.Phone
                    });
            }

            return details;
        }
    }
}
