using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Moq;
using MongoDB.Driver;
using BusTrack.Tests.MappingsIntegrationTests;
using BusTrack.BusTrack.API.ModelsAPI;
using BusTrack.BusTrack.DB.ModelsDB;

namespace BusTrack.Tests.IntegrationTests.ServicesAPIIntegrationTests
{
    public class TripsPassengerServiceAPIIntegrationTest
    {
        private Mock<ITripPassengerRepositoryDB> _tripsPassengerRepository;
        private IMapper _mapper;
        private TripsPassengerServiceAPI _tripsPassengerServiceAPI;

        public TripsPassengerServiceAPIIntegrationTest()
        {
            _tripsPassengerRepository = new Mock<ITripPassengerRepositoryDB>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            _mapper = config.CreateMapper();
            _tripsPassengerServiceAPI = new TripsPassengerServiceAPI(_tripsPassengerRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetAllTripsPassengers_ReturnsAllTripsPassengers()
        {
            List<TripPassengerDB> tripsPassengers = new List<TripPassengerDB>();
            var tripPassengerModelDBs = tripsPassengers.Select(tp => _mapper.Map<TripPassengerModelDB>(tp)).ToList();
            _tripsPassengerRepository.Setup(x => x.GetAllTripsPassengers()).Returns(Task.FromResult<IEnumerable<TripPassengerModelDB>>(tripPassengerModelDBs));

            var result = await _tripsPassengerServiceAPI.GetAllTripsPassengers();

            Assert.Equal(2, result.Count());
        }

        public async Task<TripPassengerDTOAPI> CreateTripsPassenger(TripPassengerDTOAPI tripsPassenger)
        {
            _tripsPassengerRepository.Setup(repo => repo.CreateTripsPassenger(It.IsAny<TripPassengerDB>()))
                .ReturnsAsync((TripPassengerDB tripPassengerDB) => tripPassengerDB);

            var tripPassengerModelAPI = _mapper.Map<TripsPassengerModelAPI>(tripsPassenger);

            var tripPassengerDB = ConvertToDBModel(tripPassengerModelAPI);

            var createdTripPassenger = await _tripsPassengerRepository.Object.CreateTripsPassenger(tripPassengerDB);
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

            var tripPassengerDB = ConvertToDBModel(tripPassengerModelAPI);

            var updatedTripPassenger = await _tripsPassengerRepository.Object.UpdateTripsPassenger(id, tripPassengerDB);
            return _mapper.Map<TripPassengerDTOAPI>(updatedTripPassenger);
        }

        public async Task<bool> DeleteTripsPassenger(int id)
        {
            return await _tripsPassengerRepository.Object.DeleteTripsPassenger(id);
        }


        public async Task<List<TripPassengerDB>> GetTripsPassengers()
        {
            var tripsPassengers = (await _tripsPassengerRepository.Object.GetAllTripsPassengers()).ToList();
            var mappedTripsPassengers = _mapper.Map<List<TripPassengerDB>>(tripsPassengers);
            return mappedTripsPassengers;
        }
    }
}
