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
    public class PassengerServiceAPITests
    {
        private Mock<IPassengerRepositoryDB> _passengerRepository;
        private IMapper _mapper;
        private PassengerServiceAPI _passengerServiceAPI;

        public PassengerServiceAPITests()
        {
            _passengerRepository = new Mock<IPassengerRepositoryDB>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            _mapper = config.CreateMapper();
            _passengerServiceAPI = new PassengerServiceAPI(new MongoClient(), _passengerRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetAllPassengers_ReturnsAllPassengers()
        {
            // Arrange
            var passengers = new List<PassengerDB> { new PassengerDB(), new PassengerDB() };
            _passengerRepository.Setup(x => x.GetAllPassengersAsync()).ReturnsAsync(passengers);

            // Act
            var result = await _passengerServiceAPI.GetAllPassengers();
                
            // Assert
            Assert.Equal(2, result.Count());
        }
        [Fact]
        public async Task GetPassengerById_ReturnsPassenger()
        {
            var passengerId = "passengerId";
            var passenger = new PassengerDB { Id = passengerId };
            _passengerRepository.Setup(x => x.GetPassengerByIdAsync(passengerId)).ReturnsAsync(passenger);

            var result = await _passengerServiceAPI.GetPassengerById(passengerId);

            Assert.NotNull(result);
            Assert.Equal(passengerId, result.Id);
        }


        [Fact]
        public async Task AddPassenger_ReturnsAddedPassenger()
        {
            var passengerDTO = new PassengerDTOAPI { Name = "John", Email = "john@example.com" };
            var passenger = _mapper.Map<PassengerDB>(passengerDTO);
            _passengerRepository.Setup(x => x.AddPassengerAsync(passenger)).Returns(Task.CompletedTask);

            var result = await _passengerServiceAPI.CreatePassenger(passengerDTO);

            Assert.NotNull(result);
            Assert.Equal(passengerDTO.Name, result.Name);
            Assert.Equal(passengerDTO.Email, result.Email);
        }

        [Fact]
        public async Task UpdatePassenger_ReturnsUpdatedPassenger()
        {
            var passengerId = "passengerId";
            var passengerDTO = new PassengerDTOAPI { Name = "Updated Name", Email = "updated@example.com" };
            var existingPassenger = new PassengerDB { Id = passengerId, Name = "Original Name", Email = "original@example.com" };

            _passengerRepository.Setup(x => x.GetPassengerByIdAsync(passengerId)).ReturnsAsync(existingPassenger);

            var result = await _passengerServiceAPI.UpdatePassenger(passengerId, passengerDTO);

            Assert.NotNull(result);
            Assert.Equal(passengerDTO.Name, result.Name);
            Assert.Equal(passengerDTO.Email, result.Email);
        }

        [Fact]
        public async Task DeletePassenger_ReturnsTrueWhenDeleted()
        {
            var passengerId = "passengerId";
            var existingPassenger = new PassengerDB { Id = passengerId };

            _passengerRepository.Setup(x => x.GetPassengerByIdAsync(passengerId)).ReturnsAsync(existingPassenger);

            var result = await _passengerServiceAPI.DeletePassenger(passengerId);

            Assert.True(result);
        }
    }
}