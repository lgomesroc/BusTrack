using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.Tests.MappingsIntegrationTests;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BusTrack.Tests.IntegrationTests.ServicesAPIIntegrationTests
{
    public class PassengerServiceAPITests
    {
        private readonly Mock<IPassengerRepositoryDB>
            _passengerRepository;

        private readonly IMapper _mapper;

        private readonly PassengerServiceAPI
            _passengerServiceAPI;

        public PassengerServiceAPITests()
        {
            _passengerRepository =
                new Mock<IPassengerRepositoryDB>();

            var config =
                new MapperConfiguration(
                    cfg =>
                        cfg.AddProfile<
                            AutoMapperProfile>(),
                    NullLoggerFactory.Instance);

            _mapper =
                config.CreateMapper();

            _passengerServiceAPI =
                new PassengerServiceAPI(
                    _passengerRepository.Object,
                    _mapper);
        }

        [Fact]
        public async Task GetAllPassengers_ReturnsAllPassengers()
        {
            var passengers =
                new List<PassengerDB>
                {
                    new PassengerDB(),
                    new PassengerDB()
                };

            _passengerRepository
                .Setup(x =>
                    x.GetAllPassengersAsync())
                .ReturnsAsync(
                    passengers);

            var result =
                await _passengerServiceAPI
                    .GetAllPassengers();

            Assert.Equal(
                2,
                result.Count());
        }

        [Fact]
        public async Task GetPassengerById_ReturnsPassenger()
        {
            var passengerId =
                "passengerId";

            var passenger =
                new PassengerDB
                {
                    Id = passengerId
                };

            _passengerRepository
                .Setup(x =>
                    x.GetPassengerByIdAsync(
                        passengerId))
                .ReturnsAsync(
                    passenger);

            var result =
                await _passengerServiceAPI
                    .GetPassengerById(
                        passengerId);

            Assert.NotNull(result);

            Assert.Equal(
                passengerId,
                result.Id);
        }

        [Fact]
        public async Task AddPassenger_ReturnsAddedPassenger()
        {
            var passengerDTO =
                new PassengerDTOAPI
                {
                    Name = "John",
                    Email = "john@example.com"
                };

            _passengerRepository
                .Setup(x =>
                    x.AddPassengerAsync(
                        It.IsAny<PassengerDB>()))
                .Returns(
                    Task.CompletedTask);

            var result =
                await _passengerServiceAPI
                    .CreatePassenger(
                        passengerDTO);

            Assert.NotNull(result);

            Assert.Equal(
                passengerDTO.Name,
                result.Name);

            Assert.Equal(
                passengerDTO.Email,
                result.Email);
        }

        [Fact]
        public async Task UpdatePassenger_ReturnsUpdatedPassenger()
        {
            var passengerId =
                "passengerId";

            var passengerDTO =
                new PassengerDTOAPI
                {
                    Name = "Updated Name",
                    Email = "updated@example.com"
                };

            var existingPassenger =
                new PassengerDB
                {
                    Id = passengerId,
                    Name = "Original Name",
                    Email = "original@example.com"
                };

            _passengerRepository
                .Setup(x =>
                    x.GetPassengerByIdAsync(
                        passengerId))
                .ReturnsAsync(
                    existingPassenger);

            _passengerRepository
                .Setup(x =>
                    x.UpdatePassengerAsync(
                        passengerId,
                        It.IsAny<PassengerDB>()))
                .Callback(
                    (string id, PassengerDB passenger) =>
                    {
                        existingPassenger.Name =
                            passenger.Name;

                        existingPassenger.Email =
                            passenger.Email;
                    })
                .Returns(
                    Task.CompletedTask);

            var result =
                await _passengerServiceAPI
                    .UpdatePassenger(
                        passengerId,
                        passengerDTO);

            Assert.NotNull(result);

            Assert.Equal(
                passengerDTO.Name,
                result.Name);

            Assert.Equal(
                passengerDTO.Email,
                result.Email);

            _passengerRepository.Verify(
                x =>
                    x.GetPassengerByIdAsync(
                        passengerId),
                Times.Once);

            _passengerRepository.Verify(
                x =>
                    x.UpdatePassengerAsync(
                        passengerId,
                        It.IsAny<PassengerDB>()),
                Times.Once);
        }

        [Fact]
        public async Task DeletePassenger_ReturnsTrueWhenDeleted()
        {
            var passengerId =
                "passengerId";

            _passengerRepository
                .Setup(x =>
                    x.DeletePassenger(
                        passengerId))
                .ReturnsAsync(true);

            var result =
                await _passengerServiceAPI
                    .DeletePassenger(
                        passengerId);

            Assert.True(result);

            _passengerRepository.Verify(
                x =>
                    x.DeletePassenger(
                        passengerId),
                Times.Once);
        }
    }
}
