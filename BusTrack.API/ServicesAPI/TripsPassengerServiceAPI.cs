using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class TripsPassengerServiceAPI
        : ITripsPassengerServiceAPI
    {
        private readonly ITripPassengerRepositoryDB
            _tripsPassengerRepository;

        private readonly IMapper _mapper;

        public TripsPassengerServiceAPI(
            ITripPassengerRepositoryDB
                tripsPassengerRepository,
            IMapper mapper)
        {
            _tripsPassengerRepository =
                tripsPassengerRepository;

            _mapper = mapper;
        }

        public async Task<
            IEnumerable<TripPassengerDTOAPI>>
            GetAllAsync()
        {
            var tripPassengers =
                await _tripsPassengerRepository
                    .GetAllAsync();

            return _mapper.Map<
                IEnumerable<TripPassengerDTOAPI>>(
                    tripPassengers);
        }

        public async Task<
            TripPassengerDTOAPI?>
            GetByIdAsync(
                string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var tripPassenger =
                await _tripsPassengerRepository
                    .GetByIdAsync(id);

            if (tripPassenger == null)
            {
                return null;
            }

            return _mapper.Map<
                TripPassengerDTOAPI>(
                    tripPassenger);
        }

        public async Task<
            IEnumerable<TripPassengerDTOAPI>>
            GetByTripIdAsync(
                string? tripId)
        {
            if (string.IsNullOrWhiteSpace(tripId))
            {
                return Enumerable.Empty<
                    TripPassengerDTOAPI>();
            }

            var tripPassengers =
                await _tripsPassengerRepository
                    .GetByTripIdAsync(tripId);

            return _mapper.Map<
                IEnumerable<TripPassengerDTOAPI>>(
                    tripPassengers);
        }

        public async Task<
            IEnumerable<TripPassengerDTOAPI>>
            GetByPassengerIdAsync(
                string? passengerId)
        {
            if (string.IsNullOrWhiteSpace(
                passengerId))
            {
                return Enumerable.Empty<
                    TripPassengerDTOAPI>();
            }

            var tripPassengers =
                await _tripsPassengerRepository
                    .GetByPassengerIdAsync(
                        passengerId);

            return _mapper.Map<
                IEnumerable<TripPassengerDTOAPI>>(
                    tripPassengers);
        }

        public async Task<
            TripPassengerDTOAPI>
            CreateAsync(
                TripPassengerDTOAPI tripPassenger)
        {
            var tripPassengerDB =
                _mapper.Map<TripPassengerDB>(
                    tripPassenger);

            var createdTripPassenger =
                await _tripsPassengerRepository
                    .CreateAsync(
                        tripPassengerDB);

            return _mapper.Map<
                TripPassengerDTOAPI>(
                    createdTripPassenger);
        }

        public async Task<
            TripPassengerDTOAPI?>
            UpdateAsync(
                string? id,
                TripPassengerDTOAPI tripPassenger)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var tripPassengerDB =
                _mapper.Map<TripPassengerDB>(
                    tripPassenger);

            var updatedTripPassenger =
                await _tripsPassengerRepository
                    .UpdateAsync(
                        id,
                        tripPassengerDB);

            if (updatedTripPassenger == null)
            {
                return null;
            }

            return _mapper.Map<
                TripPassengerDTOAPI>(
                    updatedTripPassenger);
        }

        public async Task<bool> DeleteAsync(
            string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            return await _tripsPassengerRepository
                .DeleteAsync(id);
        }

        public async Task<bool>
            DeleteByTripAndPassengerAsync(
                string? tripId,
                string? passengerId)
        {
            if (string.IsNullOrWhiteSpace(tripId)
                || string.IsNullOrWhiteSpace(
                    passengerId))
            {
                return false;
            }

            return await _tripsPassengerRepository
                .DeleteByTripAndPassengerAsync(
                    tripId,
                    passengerId);
        }
    }
}
