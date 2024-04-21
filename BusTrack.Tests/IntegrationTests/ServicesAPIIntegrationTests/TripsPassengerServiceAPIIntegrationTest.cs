using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Moq;
using MongoDB.Driver;
using BusTrack.Tests.MappingsIntegrationTests;
using BusTrack.BusTrack.API.ModelsAPI;

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
            // Arrange
            var tripsPassengers = new List<TripPassengerDB> { new TripPassengerDB(), new TripPassengerDB() };
            _tripsPassengerRepository.Setup(x => x.GetAllTripsPassengers()).ReturnsAsync(tripsPassengers);

            // Act
            var result = await _tripsPassengerServiceAPI.GetAllTripsPassengers();

            // Assert
            Assert.Equal(2, result.Count());
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

            var tripPassengerDB = ConvertToDBModel(tripPassengerModelAPI);

            var updatedTripPassenger = await _tripsPassengerRepository.UpdateTripsPassenger(id, tripPassengerDB);
            return _mapper.Map<TripPassengerDTOAPI>(updatedTripPassenger);
        }

        public async Task<bool> DeleteTripsPassenger(int id)
        {
            return await _tripsPassengerRepository.DeleteTripsPassenger(id);
        }

        public async Task<List<TripPassengerDB>> GetTripsPassengers()
        {
            var tripsPassengers = (await _tripsPassengerRepository.GetAllTripsPassengers()).ToList();
            var mappedTripsPassengers = _mapper.Map<List<TripPassengerDB>>(tripsPassengers);
            return mappedTripsPassengers;
        }
    }
}
