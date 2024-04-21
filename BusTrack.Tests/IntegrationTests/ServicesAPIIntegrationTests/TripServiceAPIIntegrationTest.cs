using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Moq;
using MongoDB.Driver;
using BusTrack.Tests.MappingsIntegrationTests;

namespace BusTrack.Tests.IntegrationTests.ServicesAPIIntegrationTests
{
    public class TripServiceAPIIntegrationTest
    {
        private Mock<ITripRepositoryDB> _tripRepository;
        private IMapper _mapper;
        private TripServiceAPI _tripServiceAPI;

        public TripServiceAPIIntegrationTest()
        {
            _tripRepository = new Mock<ITripRepositoryDB>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            _mapper = config.CreateMapper();
            _tripServiceAPI = new TripServiceAPI(_tripRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetAllTrips_ReturnsAllTrips()
        {
            // Arrange
            var trips = new List<TripDB> { new TripDB(), new TripDB() };
            _tripRepository.Setup(x => x.GetAllTrips()).ReturnsAsync(trips);

            // Act
            var result = await _tripServiceAPI.GetAllTrips();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetTripById_ReturnsTrip()
        {
            // Arrange
            var tripId = "tripId";
            var trip = new TripDB { Id = tripId };
            _tripRepository.Setup(x => x.GetTripById(tripId)).ReturnsAsync(trip);

            // Act
            var result = await _tripServiceAPI.GetTripById(tripId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tripId, result.Id);
        }

        [Fact]
        public async Task CreateTrip_ReturnsAddedTrip()
        {
            // Arrange
            var tripDTO = new TripDTOAPI { BusId = 1, DriverId = 1, RouteId = 1, DepartureTime = DateTime.UtcNow };
            var tripDB = _mapper.Map<TripDB>(tripDTO);
            _tripRepository.Setup(x => x.AddTripAsync(It.IsAny<TripDB>())).ReturnsAsync(tripDB);

            // Act
            var result = await _tripServiceAPI.CreateTrip(tripDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tripDTO.BusId, result.BusId);
            Assert.Equal(tripDTO.DriverId, result.DriverId);
            Assert.Equal(tripDTO.RouteId, result.RouteId);
            Assert.Equal(tripDTO.DepartureTime, result.DepartureTime);
        }

        [Fact]
        public async Task UpdateTrip_ReturnsUpdatedTrip()
        {
            // Arrange
            var tripId = "tripId";
            var tripDTO = new TripDTOAPI { BusId = 1, DriverId = 1, RouteId = 1, DepartureTime = DateTime.UtcNow };
            var existingTrip = new TripDB { Id = tripId };

            _tripRepository.Setup(x => x.UpdateTripAsync(tripId, It.IsAny<TripDB>())).ReturnsAsync(existingTrip);

            // Act
            var result = await _tripServiceAPI.UpdateTrip(tripId, tripDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tripDTO.BusId, result.BusId);
            Assert.Equal(tripDTO.DriverId, result.DriverId);
            Assert.Equal(tripDTO.RouteId, result.RouteId);
            Assert.Equal(tripDTO.DepartureTime, result.DepartureTime);
        }

        [Fact]
        public async Task DeleteTrip_ReturnsTrueWhenDeleted()
        {
            // Arrange
            var tripId = "tripId";
            _tripRepository.Setup(x => x.DeleteTripAsync(tripId)).ReturnsAsync(true);

            // Act
            var result = await _tripServiceAPI.DeleteTrip(tripId);

            // Assert
            Assert.True(result);
        }
    }

}
