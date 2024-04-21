using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.Tests.MappingsIntegrationTests;
using Moq;

namespace BusTrack.Tests.IntegrationTests.ServicesAPIIntegrationTests
{
    public class BusServiceAPIIntegrationTest
    {
        private Mock<IBusRepositoryDB> _busRepository;
        private IMapper _mapper;
        private BusServiceAPI _busServiceAPI;

        public BusServiceAPIIntegrationTest()
        {
            _busRepository = new Mock<IBusRepositoryDB>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            _mapper = config.CreateMapper();
            _busServiceAPI = new BusServiceAPI(_busRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetAllBuses_ReturnsAllBuses()
        {
            // Arrange
            var buses = new List<BusDB> { new BusDB(), new BusDB() };
            _busRepository.Setup(x => x.GetAllBusesAsync()).ReturnsAsync(buses);

            // Act
            var result = await _busServiceAPI.GetAllBuses();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllBuses_ShouldCallRepositoryMethod()
        {
            // Arrange
            var buses = new List<BusDB> { new BusDB(), new BusDB() };
            _busRepository.Setup(x => x.GetAllBusesAsync()).ReturnsAsync(buses);

            // Act
            await _busServiceAPI.GetAllBuses();

            // Assert
            _busRepository.Verify(x => x.GetAllBusesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetBusById_ReturnsBusWhenFound()
        {
            // Arrange
            var existingBusId = "123";
            var existingBus = new BusDB { Id = existingBusId, Plate = "DEF456", Line = "Nova Iguaçu x Barra" };
            _busRepository.Setup(x => x.GetBusByIdAsync(existingBusId)).ReturnsAsync(existingBus);

            // Act
            var result = await _busServiceAPI.GetBusById(existingBusId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingBusId, result.Id.ToString());
        }

        [Fact]
        public async Task GetBusById_ReturnsNullWhenNotFound()
        {
            // Arrange
            var nonExistingBusId = "999";
            _busRepository.Setup(x => x.GetBusByIdAsync(nonExistingBusId)).ReturnsAsync(() => null);

            // Act
            var result = await _busServiceAPI.GetBusById(nonExistingBusId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateBus_CreatesAndMapsBus()
        {
            // Arrange
            var newBusDto = new BusDTOAPI { LicensePlate = "ABC123", Model = "Modelo X" };
            var expectedBusId = "new-bus-id";

            _busRepository.Setup(x => x.CreateBus(It.IsAny<BusDB>())).ReturnsAsync(new BusDB { Id = expectedBusId });

            // Act
            var createdBusDto = await _busServiceAPI.CreateBus(newBusDto);

            // Assert
            _busRepository.Verify(x => x.CreateBus(It.IsAny<BusDB>()), Times.Once);
            Assert.NotNull(createdBusDto);
            Assert.Equal(expectedBusId, createdBusDto.Id);
            Assert.Equal(newBusDto.LicensePlate, createdBusDto.LicensePlate);
            Assert.Equal(newBusDto.Model, createdBusDto.Model);
        }

        [Fact]
        public async Task DeleteBus_ReturnsFalseWhenNotFound()
        {
            // Arrange
            var nonExistingBusId = "999";
            _busRepository.Setup(x => x.DeleteBus(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _busServiceAPI.DeleteBus(nonExistingBusId);

            // Assert
            _busRepository.Verify(x => x.GetBusByIdAsync(nonExistingBusId), Times.Once);
            _busRepository.Verify(x => x.DeleteBus(It.IsAny<string>()), Times.Never);
            Assert.False(result);
        }
    }
}