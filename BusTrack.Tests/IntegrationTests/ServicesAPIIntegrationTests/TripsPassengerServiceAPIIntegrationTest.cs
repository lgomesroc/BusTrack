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
    public class TripsPassengerServiceAPIIntegrationTest
    {
        private readonly Mock<ITripPassengerRepositoryDB>
            _tripsPassengerRepository;

        private readonly IMapper _mapper;

        private readonly TripsPassengerServiceAPI
            _tripsPassengerServiceAPI;

        public TripsPassengerServiceAPIIntegrationTest()
        {
            _tripsPassengerRepository =
                new Mock<ITripPassengerRepositoryDB>();

            var config = new MapperConfiguration(
                cfg =>
                {
                    cfg.AddProfile<AutoMapperProfile>();
                },
                NullLoggerFactory.Instance);

            _mapper = config.CreateMapper();

            _tripsPassengerServiceAPI =
                new TripsPassengerServiceAPI(
                    _tripsPassengerRepository.Object,
                    _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTripsPassengers()
        {
            var tripsPassengers =
                new List<TripPassengerDB>
                {
                    new TripPassengerDB
                    {
                        Id = "507f1f77bcf86cd799439011",
                        TripId = "507f1f77bcf86cd799439012",
                        PassengerId = "507f1f77bcf86cd799439013"
                    },
                    new TripPassengerDB
                    {
                        Id = "507f1f77bcf86cd799439014",
                        TripId = "507f1f77bcf86cd799439015",
                        PassengerId = "507f1f77bcf86cd799439016"
                    }
                };

            _tripsPassengerRepository
                .Setup(repository => repository.GetAllAsync())
                .ReturnsAsync(tripsPassengers);

            var result =
                await _tripsPassengerServiceAPI.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            _tripsPassengerRepository.Verify(
                repository => repository.GetAllAsync(),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTripPassengerExists_ReturnsTripPassenger()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            var tripPassenger =
                new TripPassengerDB
                {
                    Id = id,
                    TripId = "507f1f77bcf86cd799439012",
                    PassengerId = "507f1f77bcf86cd799439013"
                };

            _tripsPassengerRepository
                .Setup(repository =>
                    repository.GetByIdAsync(id))
                .ReturnsAsync(tripPassenger);

            var result =
                await _tripsPassengerServiceAPI.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result!.Id);
            Assert.Equal(
                tripPassenger.TripId,
                result.TripId);
            Assert.Equal(
                tripPassenger.PassengerId,
                result.PassengerId);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTripPassengerDoesNotExist_ReturnsNull()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            _tripsPassengerRepository
                .Setup(repository =>
                    repository.GetByIdAsync(id))
                .ReturnsAsync(
                    (TripPassengerDB?)null);

            var result =
                await _tripsPassengerServiceAPI.GetByIdAsync(id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByTripIdAsync_ReturnsTripPassengers()
        {
            const string tripId =
                "507f1f77bcf86cd799439012";

            var tripsPassengers =
                new List<TripPassengerDB>
                {
                    new TripPassengerDB
                    {
                        Id = "507f1f77bcf86cd799439011",
                        TripId = tripId,
                        PassengerId = "507f1f77bcf86cd799439013"
                    }
                };

            _tripsPassengerRepository
                .Setup(repository =>
                    repository.GetByTripIdAsync(tripId))
                .ReturnsAsync(tripsPassengers);

            var result =
                await _tripsPassengerServiceAPI
                    .GetByTripIdAsync(tripId);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(
                tripId,
                result.First().TripId);
        }

        [Fact]
        public async Task GetByPassengerIdAsync_ReturnsTripPassengers()
        {
            const string passengerId =
                "507f1f77bcf86cd799439013";

            var tripsPassengers =
                new List<TripPassengerDB>
                {
                    new TripPassengerDB
                    {
                        Id = "507f1f77bcf86cd799439011",
                        TripId = "507f1f77bcf86cd799439012",
                        PassengerId = passengerId
                    }
                };

            _tripsPassengerRepository
                .Setup(repository =>
                    repository.GetByPassengerIdAsync(
                        passengerId))
                .ReturnsAsync(tripsPassengers);

            var result =
                await _tripsPassengerServiceAPI
                    .GetByPassengerIdAsync(
                        passengerId);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(
                passengerId,
                result.First().PassengerId);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedTripPassenger()
        {
            var tripPassenger =
                new TripPassengerDTOAPI
                {
                    TripId = "507f1f77bcf86cd799439012",
                    PassengerId = "507f1f77bcf86cd799439013"
                };

            _tripsPassengerRepository
                .Setup(repository =>
                    repository.CreateAsync(
                        It.IsAny<TripPassengerDB>()))
                .ReturnsAsync(
                    (TripPassengerDB tripPassengerDB) =>
                    {
                        tripPassengerDB.Id =
                            "507f1f77bcf86cd799439011";

                        return tripPassengerDB;
                    });

            var result =
                await _tripsPassengerServiceAPI
                    .CreateAsync(tripPassenger);

            Assert.NotNull(result);
            Assert.NotNull(result.Id);

            Assert.Equal(
                tripPassenger.TripId,
                result.TripId);

            Assert.Equal(
                tripPassenger.PassengerId,
                result.PassengerId);

            _tripsPassengerRepository.Verify(
                repository =>
                    repository.CreateAsync(
                        It.IsAny<TripPassengerDB>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenTripPassengerExists_ReturnsUpdatedTripPassenger()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            var tripPassenger =
                new TripPassengerDTOAPI
                {
                    Id = id,
                    TripId = "507f1f77bcf86cd799439012",
                    PassengerId = "507f1f77bcf86cd799439013"
                };

            _tripsPassengerRepository
                .Setup(repository =>
                    repository.UpdateAsync(
                        id,
                        It.IsAny<TripPassengerDB>()))
                .ReturnsAsync(
                    (string _,
                        TripPassengerDB tripPassengerDB) =>
                    {
                        tripPassengerDB.Id = id;

                        return tripPassengerDB;
                    });

            var result =
                await _tripsPassengerServiceAPI
                    .UpdateAsync(
                        id,
                        tripPassenger);

            Assert.NotNull(result);
            Assert.Equal(id, result!.Id);

            Assert.Equal(
                tripPassenger.TripId,
                result.TripId);

            Assert.Equal(
                tripPassenger.PassengerId,
                result.PassengerId);
        }

        [Fact]
        public async Task UpdateAsync_WhenTripPassengerDoesNotExist_ReturnsNull()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            var tripPassenger =
                new TripPassengerDTOAPI
                {
                    Id = id,
                    TripId = "507f1f77bcf86cd799439012",
                    PassengerId = "507f1f77bcf86cd799439013"
                };

            _tripsPassengerRepository
                .Setup(repository =>
                    repository.UpdateAsync(
                        id,
                        It.IsAny<TripPassengerDB>()))
                .ReturnsAsync(
                    (TripPassengerDB?)null);

            var result =
                await _tripsPassengerServiceAPI
                    .UpdateAsync(
                        id,
                        tripPassenger);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_WhenTripPassengerExists_ReturnsTrue()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            _tripsPassengerRepository
                .Setup(repository =>
                    repository.DeleteAsync(id))
                .ReturnsAsync(true);

            var result =
                await _tripsPassengerServiceAPI
                    .DeleteAsync(id);

            Assert.True(result);

            _tripsPassengerRepository.Verify(
                repository =>
                    repository.DeleteAsync(id),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenTripPassengerDoesNotExist_ReturnsFalse()
        {
            const string id =
                "507f1f77bcf86cd799439011";

            _tripsPassengerRepository
                .Setup(repository =>
                    repository.DeleteAsync(id))
                .ReturnsAsync(false);

            var result =
                await _tripsPassengerServiceAPI
                    .DeleteAsync(id);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteByTripAndPassengerAsync_WhenAssociationExists_ReturnsTrue()
        {
            const string tripId =
                "507f1f77bcf86cd799439012";

            const string passengerId =
                "507f1f77bcf86cd799439013";

            _tripsPassengerRepository
                .Setup(repository =>
                    repository.DeleteByTripAndPassengerAsync(
                        tripId,
                        passengerId))
                .ReturnsAsync(true);

            var result =
                await _tripsPassengerServiceAPI
                    .DeleteByTripAndPassengerAsync(
                        tripId,
                        passengerId);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteByTripAndPassengerAsync_WhenAssociationDoesNotExist_ReturnsFalse()
        {
            const string tripId =
                "507f1f77bcf86cd799439012";

            const string passengerId =
                "507f1f77bcf86cd799439013";

            _tripsPassengerRepository
                .Setup(repository =>
                    repository.DeleteByTripAndPassengerAsync(
                        tripId,
                        passengerId))
                .ReturnsAsync(false);

            var result =
                await _tripsPassengerServiceAPI
                    .DeleteByTripAndPassengerAsync(
                        tripId,
                        passengerId);

            Assert.False(result);
        }
    }
}
