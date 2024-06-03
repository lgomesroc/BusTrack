using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ModelsAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class TripServiceAPI : ITripServiceAPI
    {
        private readonly ITripRepositoryDB _tripRepository;
        private readonly IMapper _mapper;

        public TripServiceAPI(ITripRepositoryDB tripRepository, IMapper mapper)
        {
            _tripRepository = tripRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TripDTOAPI>> GetAllTrips()
        {
            var trips = await _tripRepository.GetAllTrips();
            return _mapper.Map<IEnumerable<TripDTOAPI>>(trips);
        }

        public async Task<TripDTOAPI> GetTripById(int id)
        {
            var trip = await _tripRepository.GetTripById(id);
            return _mapper.Map<TripDTOAPI>(trip);
        }

        public async Task<TripDTOAPI> CreateTrip(TripDTOAPI trip)
        {
            var tripDB = _mapper.Map<TripDB>(trip);
            await _tripRepository.AddTripAsync(tripDB);
            return _mapper.Map<TripDTOAPI>(tripDB);
        }

        public async Task<TripDTOAPI> UpdateTrip(int id, TripDTOAPI trip)
        {
            var tripModelAPI = _mapper.Map<TripModelAPI>(trip);

            var tripDB = ConvertToDBModel(tripModelAPI);

            await _tripRepository.UpdateTripAsync(id.ToString(), tripDB);
            var updatedTrip = await _tripRepository.GetTripByIdAsync(id.ToString());
            return _mapper.Map<TripDTOAPI>(updatedTrip);
        }

        private TripDB ConvertToDBModel(TripModelAPI modelAPI)
        {
            var dbModel = new TripDB();

            dbModel.Id = modelAPI.Id.ToString();
            dbModel.BusId = modelAPI.BusId.ToString();
            dbModel.DriverId = modelAPI.DriverId.ToString();
            dbModel.DepartureTime = modelAPI.DepartureTime;

            return dbModel;
        }

        public async Task<bool> DeleteTrip(int id)
        {
            return await _tripRepository.DeleteTripAsync(id.ToString());
        }

        public async Task<List<TripDB>> GetTrip()
        {
            var trip = (await _tripRepository.GetAllTripsAsync()).ToList();
            return trip;
        }
    }
}
