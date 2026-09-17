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
        private readonly IMapper _mapper;

        public TripServiceAPI(
            ITripRepositoryDB tripRepository,
            IMapper mapper)
        {
            _tripRepository = tripRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TripDTOAPI>> GetAllTripsAsync()
        {
            var trips =
                await _tripRepository.GetAllTripsAsync();

            return _mapper.Map<IEnumerable<TripDTOAPI>>(
                trips);
        }

        public async Task<TripDTOAPI?> GetTripByIdAsync(
            string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var trip =
                await _tripRepository.GetTripByIdAsync(id);

            if (trip == null)
            {
                return null;
            }

            return _mapper.Map<TripDTOAPI>(trip);
        }

        public async Task<TripDTOAPI> CreateTripAsync(
            TripDTOAPI trip)
        {
            var tripDB =
                _mapper.Map<TripDB>(trip);

            var createdTrip =
                await _tripRepository.AddTripAsync(
                    tripDB);

            return _mapper.Map<TripDTOAPI>(
                createdTrip);
        }

        public async Task<TripDTOAPI?> UpdateTripAsync(
            string? id,
            TripDTOAPI trip)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var existingTrip =
                await _tripRepository.GetTripByIdAsync(id);

            if (existingTrip == null)
            {
                return null;
            }

            _mapper.Map(
                trip,
                existingTrip);

            existingTrip.Id = id;

            var updatedTrip =
                await _tripRepository.UpdateTripAsync(
                    id,
                    existingTrip);

            if (updatedTrip == null)
            {
                return null;
            }

            return _mapper.Map<TripDTOAPI>(
                updatedTrip);
        }

        public async Task<bool> DeleteTripAsync(
            string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            return await _tripRepository
                .DeleteTripAsync(id);
        }
    }
}
