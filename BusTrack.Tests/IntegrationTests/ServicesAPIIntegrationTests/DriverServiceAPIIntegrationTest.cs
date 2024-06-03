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
    public class DriverServiceAPIIntegrationTest
    {
        private readonly Mock<IDriverRepositoryDB> _driverRepository;
        private readonly IMapper _mapper;
        private readonly DriverServiceAPI _driverServiceAPI;

        public DriverServiceAPIIntegrationTest()
        {
            _driverRepository = new Mock<IDriverRepositoryDB>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            _mapper = config.CreateMapper();
            _driverServiceAPI = new DriverServiceAPI(new MongoClient(), _driverRepository.Object, _mapper);
        }

        [Fact]
        public async Task GetAllDrivers_ReturnsAllDrivers()
        {
            var drivers = new List<DriverDB> { new DriverDB(), new DriverDB() };
            _driverRepository.Setup(x => x.GetAllDriversAsync()).ReturnsAsync(drivers.AsEnumerable());

            var result = await _driverServiceAPI.GetAllDrivers();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetDriverById_ReturnsDriverWhenFound()
        {
            var existingDriverId = "123";
            var existingDriver = new DriverDB { Id = existingDriverId, Name = "John Doe", Cpf = "000000000-00" };
            _driverRepository.Setup(x => x.GetDriverByIdAsync(existingDriverId)).ReturnsAsync(existingDriver);

            var result = await _driverServiceAPI.GetDriverById(existingDriverId);

            Assert.NotNull(result);
            Assert.Equal(existingDriverId, result.Id);
            Assert.Equal(existingDriver.Name, result.Name);
            Assert.Equal(existingDriver.Cpf, result.LicenseNumber);
        }

        [Fact]
        public async Task GetDriverById_ReturnsNullWhenNotFound()
        {
            var nonExistingDriverId = "999";
            _driverRepository.Setup(x => x.GetDriverByIdAsync(nonExistingDriverId)).ReturnsAsync(() => null);

            var result = await _driverServiceAPI.GetDriverById(nonExistingDriverId);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateDriver_CreatesAndMapsDriver()
        {
            var newDriverDto = new DriverDTOAPI { Name = "Jane Doe", LicenseNumber = "DEF54321" };
            var expectedDriverId = "new-driver-id";

            _driverRepository.Setup(x => x.AddDriverAsync(It.IsAny<DriverDB>())).Returns((DriverDB driver) =>
            {
                driver.Id = expectedDriverId;
                return Task.FromResult(driver);
            });

            var addedDriverDto = await _driverServiceAPI.CreateDriver(newDriverDto);

            _driverRepository.Verify(x => x.AddDriverAsync(It.IsAny<DriverDB>()), Times.Once);
            Assert.NotNull(addedDriverDto);
            Assert.Equal(expectedDriverId, addedDriverDto.Id);
            Assert.Equal(newDriverDto.Name, addedDriverDto.Name);
            Assert.Equal(newDriverDto.LicenseNumber, addedDriverDto.LicenseNumber);
        }


        [Fact]
        public async Task UpdateDriver_UpdatesAndMapsDriver()
        {
            var existingDriverId = "123";
            var existingDriver = new DriverDB { Id = existingDriverId, Name = "John Doe", Cpf = "ABC12345" };
            var updateDriverDto = new DriverDTOAPI { Name = "John Smith", LicenseNumber = "FED65432" };

            _driverRepository.Setup(x => x.GetDriverByIdAsync(existingDriverId)).ReturnsAsync(existingDriver);
            _driverRepository.Setup(x => x.UpdateDriverAsync(existingDriverId, It.IsAny<DriverDB>())).Returns(Task.FromResult(true));

            var updatedDriverDto = await _driverServiceAPI.UpdateDriver(existingDriverId, updateDriverDto);

            _driverRepository.Verify(x => x.GetDriverByIdAsync(existingDriverId), Times.Once);
            _driverRepository.Verify(x => x.UpdateDriverAsync(existingDriverId, It.IsAny<DriverDB>()), Times.Once);
            Assert.NotNull(updatedDriverDto);
            Assert.Equal(existingDriverId, updatedDriverDto.Id);
            Assert.Equal(updateDriverDto.Name, updatedDriverDto.Name);
            Assert.Equal(updateDriverDto.LicenseNumber, updatedDriverDto.LicenseNumber);
        }

        [Fact]
        public async Task DeleteDriver_DeletesDriverAndReturnsTrueWhenFound()
        {
            var existingDriverId = "123";
            _driverRepository.Setup(x => x.GetDriverByIdAsync(existingDriverId)).ReturnsAsync(new DriverDB { Id = existingDriverId });
            _driverRepository.Setup(x => x.DeleteDriver(existingDriverId)).ReturnsAsync(true);
            var result = await _driverServiceAPI.DeleteDriver(existingDriverId);

            _driverRepository.Verify(x => x.GetDriverByIdAsync(existingDriverId), Times.Once);
            _driverRepository.Verify(x => x.DeleteDriverAsync(existingDriverId), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteDriver_ReturnsFalseWhenNotFound()
        {
            var nonExistingDriverId = "999";
            _driverRepository.Setup(x => x.DeleteDriver(nonExistingDriverId)).ReturnsAsync(true);

            var result = await _driverServiceAPI.DeleteDriver(nonExistingDriverId);

            _driverRepository.Verify(x => x.GetDriverByIdAsync(nonExistingDriverId), Times.Once);
            _driverRepository.Verify(x => x.DeleteDriverAsync(nonExistingDriverId), Times.Never);
            Assert.False(result);
        }
    }
}

