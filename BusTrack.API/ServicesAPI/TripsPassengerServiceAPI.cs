using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using BusTrack.BusTrack.API.ModelsAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class TripsPassengerServiceAPI : ITripsPassengerServiceAPI
    {
        private readonly ITripPassengerRepositoryDB _tripsPassengerRepository;
        private readonly IMapper _mapper;

        public TripsPassengerServiceAPI(ITripPassengerRepositoryDB tripsPassengerRepository, IMapper mapper)
        {
            _tripsPassengerRepository = tripsPassengerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TripPassengerDTOAPI>> GetAllTripsPassengers()
        {
            var tripsPassengers = await _tripsPassengerRepository.GetAllTripsPassengers();
            return _mapper.Map<IEnumerable<TripPassengerDTOAPI>>(tripsPassengers);
        }

        public async Task<TripPassengerDTOAPI> GetTripsPassengerById(int id)
        {
            var tripPassenger = await _tripsPassengerRepository.GetTripsPassengerById(id);
            return _mapper.Map<TripPassengerDTOAPI>(tripPassenger);
        }

        public async Task<TripPassengerDTOAPI> CreateTripsPassenger(TripPassengerDTOAPI tripsPassenger)
        {
            var tripPassengerModelAPI = _mapper.Map<TripsPassengerModelAPI>(tripsPassenger);

            var tripPassengerDB = ConvertToDBModel(tripPassengerModelAPI);

            var createdTripPassenger = await _tripsPassengerRepository.CreateTripsPassenger(tripPassengerDB);
            return _mapper.Map<TripPassengerDTOAPI>(createdTripPassenger);
        }

        private TripPassengerDB ConvertToDBModel(TripsPassengerModelAPI modelAPI)
        {
            var dbModel = new TripPassengerDB();

            dbModel.TripId = modelAPI.TripId;
            dbModel.PassengerId = modelAPI.PassengerId;

            return dbModel;
        }

        public async Task<TripPassengerDTOAPI> UpdateTripsPassenger(int id, TripPassengerDTOAPI tripsPassenger)
        {
            var tripPassengerModelAPI = _mapper.Map<TripsPassengerModelAPI>(tripsPassenger);

            // Converta o TripsPassengerModelAPI para TripPassengerDB aqui
            var tripPassengerDB = ConvertToDBModel1(tripPassengerModelAPI);

            var updatedTripPassenger = await _tripsPassengerRepository.UpdateTripsPassenger(id, tripPassengerDB);
            return _mapper.Map<TripPassengerDTOAPI>(updatedTripPassenger);
        }

        private TripPassengerDB ConvertToDBModel1(TripsPassengerModelAPI modelAPI)
        {
            var dbModel = new TripPassengerDB();

            // Copie os campos do modelAPI para dbModel
            dbModel.TripId = modelAPI.TripId;
            dbModel.PassengerId = modelAPI.PassengerId;

            return dbModel;
        }

        public async Task<bool> DeleteTripsPassenger(int id)
        {
            return await _tripsPassengerRepository.DeleteTripsPassenger(id);
        }
    }
}
