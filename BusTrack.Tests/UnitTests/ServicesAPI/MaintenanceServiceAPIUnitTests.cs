
using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.MappingsAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BusTrack.Tests.UnitTests.ServicesAPI;

public class MaintenanceServiceAPIUnitTests
{
    private const string MaintenanceId = "507f1f77bcf86cd799439011";
    private const string BusId = "507f1f77bcf86cd799439012";
    private const string InvalidId = "id-invalido";

    private readonly Mock<IMaintenanceRepositoryDB> _repository = new();
    private readonly Mock<IBusRepositoryDB> _busRepository = new();
    private readonly MaintenanceServiceAPI _service;

    public MaintenanceServiceAPIUnitTests()
    {
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<MappingProfileAPI>(),
            NullLoggerFactory.Instance);

        _service = new MaintenanceServiceAPI(
            _repository.Object,
            _busRepository.Object,
            configuration.CreateMapper());
    }

    [Fact]
    public async Task GetAllAsync_WhenNoMaintenances_ReturnsEmptyCollection()
    {
        _repository
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(new List<MaintenanceDB>());

        var result = await _service.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);

        _repository.Verify(
            repository => repository.GetAllAsync(),
            Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenMaintenancesExist_ReturnsMappedItems()
    {
        _repository
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(new List<MaintenanceDB>
            {
                new()
                {
                    Id = MaintenanceId,
                    BusId = BusId,
                    BusNumber = "001",
                    ServiceType = "Revisão",
                    Description = "Revisão preventiva",
                    ServiceDate = DateTime.UtcNow.AddDays(2),
                    Cost = 500m,
                    Status = "Agendada"
                }
            });

        var result = (await _service.GetAllAsync()).ToList();

        var maintenance = Assert.Single(result);

        Assert.Equal(MaintenanceId, maintenance.Id);
        Assert.Equal(BusId, maintenance.BusId);
        Assert.Equal("001", maintenance.BusNumber);
        Assert.Equal("Revisão", maintenance.ServiceType);
        Assert.Equal(500m, maintenance.Cost);
    }

    [Fact]
    public async Task GetByIdAsync_WhenIdIsInvalid_ReturnsNullWithoutRepositoryCall()
    {
        var result = await _service.GetByIdAsync(InvalidId);

        Assert.Null(result);

        _repository.Verify(
            repository => repository.GetByIdAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMaintenanceDoesNotExist_ReturnsNull()
    {
        _repository
            .Setup(repository => repository.GetByIdAsync(MaintenanceId))
            .ReturnsAsync((MaintenanceDB?)null);

        var result = await _service.GetByIdAsync(MaintenanceId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMaintenanceExists_ReturnsMappedItem()
    {
        _repository
            .Setup(repository => repository.GetByIdAsync(MaintenanceId))
            .ReturnsAsync(new MaintenanceDB
            {
                Id = MaintenanceId,
                BusId = BusId,
                ServiceType = "Revisão",
                Description = "Revisão preventiva",
                ServiceDate = DateTime.UtcNow.AddDays(2),
                Cost = 250m,
                Status = "Agendada"
            });

        var result = await _service.GetByIdAsync(MaintenanceId);

        Assert.NotNull(result);
        Assert.Equal(MaintenanceId, result.Id);
        Assert.Equal("Revisão", result.ServiceType);
        Assert.Equal(250m, result.Cost);
    }

    [Fact]
    public async Task CreateAsync_WhenDataIsValid_CreatesMaintenance()
    {
        SetupExistingBus();

        _repository
            .Setup(repository => repository.CreateAsync(
                It.IsAny<MaintenanceDB>()))
            .ReturnsAsync((MaintenanceDB maintenance) =>
            {
                maintenance.Id = MaintenanceId;
                return maintenance;
            });

        var maintenance = CreateValidMaintenance();
        maintenance.ServiceType = "  Revisão  ";
        maintenance.Description = "  Revisão preventiva  ";

        var result = await _service.CreateAsync(maintenance);

        Assert.NotNull(result);
        Assert.Equal(MaintenanceId, result.Id);
        Assert.Equal("Revisão", result.ServiceType);
        Assert.Equal("Revisão preventiva", result.Description);
        Assert.Equal(BusId, result.BusId);
        Assert.Equal("001", result.BusNumber);
        Assert.Equal("Agendada", result.Status);

        _repository.Verify(
            repository => repository.CreateAsync(
                It.IsAny<MaintenanceDB>()),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_WhenServiceTypeIsBlank_ReturnsNull(
        string serviceType)
    {
        var maintenance = CreateValidMaintenance();
        maintenance.ServiceType = serviceType;

        var result = await _service.CreateAsync(maintenance);

        Assert.Null(result);

        _repository.Verify(
            repository => repository.CreateAsync(
                It.IsAny<MaintenanceDB>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenDescriptionExceedsLimit_ReturnsNull()
    {
        var maintenance = CreateValidMaintenance();
        maintenance.Description = new string('A', 1001);

        var result = await _service.CreateAsync(maintenance);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenCostIsNegative_ReturnsNull()
    {
        var maintenance = CreateValidMaintenance();
        maintenance.Cost = -1m;

        var result = await _service.CreateAsync(maintenance);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenCostHasMoreThanTwoDecimalPlaces_ReturnsNull()
    {
        var maintenance = CreateValidMaintenance();
        maintenance.Cost = 10.999m;

        var result = await _service.CreateAsync(maintenance);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenStatusIsInvalid_ReturnsNull()
    {
        var maintenance = CreateValidMaintenance();
        maintenance.Status = "Status inválido";

        var result = await _service.CreateAsync(maintenance);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenBusDoesNotExist_ReturnsNull()
    {
        _busRepository
            .Setup(repository => repository.GetBusByIdAsync(BusId))
            .ReturnsAsync((BusDB?)null);

        var result = await _service.CreateAsync(CreateValidMaintenance());

        Assert.Null(result);

        _repository.Verify(
            repository => repository.CreateAsync(
                It.IsAny<MaintenanceDB>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenBusIdIsInvalid_ReturnsNull()
    {
        var maintenance = CreateValidMaintenance();
        maintenance.BusId = InvalidId;

        var result = await _service.CreateAsync(maintenance);

        Assert.Null(result);

        _busRepository.Verify(
            repository => repository.GetBusByIdAsync(
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenScheduledDateIsInThePast_ReturnsNull()
    {
        var maintenance = CreateValidMaintenance();
        maintenance.ServiceDate = DateTime.UtcNow.AddDays(-3);
        maintenance.Status = "Agendada";

        var result = await _service.CreateAsync(maintenance);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenServiceDateIsDefault_ReturnsNull()
    {
        var maintenance = CreateValidMaintenance();
        maintenance.ServiceDate = default;

        var result = await _service.CreateAsync(maintenance);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenStatusIsCompleted_SetsCompletedDate()
    {
        SetupExistingBus();

        _repository
            .Setup(repository => repository.CreateAsync(
                It.IsAny<MaintenanceDB>()))
            .ReturnsAsync((MaintenanceDB maintenance) =>
            {
                maintenance.Id = MaintenanceId;
                return maintenance;
            });

        var maintenance = CreateValidMaintenance();
        maintenance.ServiceDate = DateTime.UtcNow.AddDays(-2);
        maintenance.Status = "Concluída";

        var result = await _service.CreateAsync(maintenance);

        Assert.NotNull(result);
        Assert.Equal("Concluída", result.Status);
        Assert.NotNull(result.CompletedDate);
        Assert.True(result.CompletedDate.Value <= DateTime.UtcNow);
    }

    [Fact]
    public async Task UpdateAsync_WhenIdIsInvalid_ReturnsNull()
    {
        var result = await _service.UpdateAsync(
            InvalidId,
            CreateValidMaintenance());

        Assert.Null(result);

        _repository.Verify(
            repository => repository.GetByIdAsync(
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenMaintenanceDoesNotExist_ReturnsNull()
    {
        _repository
            .Setup(repository => repository.GetByIdAsync(MaintenanceId))
            .ReturnsAsync((MaintenanceDB?)null);

        var result = await _service.UpdateAsync(
            MaintenanceId,
            CreateValidMaintenance());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenTransitionIsValid_UpdatesMaintenance()
    {
        SetupExistingBus();

        var existing = new MaintenanceDB
        {
            Id = MaintenanceId,
            BusId = BusId,
            BusNumber = "001",
            ServiceType = "Revisão",
            Description = "Revisão inicial",
            ServiceDate = DateTime.UtcNow.AddDays(-1),
            Cost = 100m,
            Status = "Agendada"
        };

        _repository
            .Setup(repository => repository.GetByIdAsync(MaintenanceId))
            .ReturnsAsync(existing);

        _repository
            .Setup(repository => repository.UpdateAsync(
                MaintenanceId,
                It.IsAny<MaintenanceDB>()))
            .ReturnsAsync(true);

        var maintenance = CreateValidMaintenance();
        maintenance.ServiceDate = DateTime.UtcNow.AddDays(-1);
        maintenance.Status = "Em andamento";
        maintenance.Description = "Revisão iniciada";

        var result = await _service.UpdateAsync(
            MaintenanceId,
            maintenance);

        Assert.NotNull(result);
        Assert.Equal("Em andamento", result.Status);
        Assert.Equal("Revisão iniciada", result.Description);

        _repository.Verify(
            repository => repository.UpdateAsync(
                MaintenanceId,
                It.IsAny<MaintenanceDB>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenMaintenanceIsCompleted_ReturnsNull()
    {
        _repository
            .Setup(repository => repository.GetByIdAsync(MaintenanceId))
            .ReturnsAsync(new MaintenanceDB
            {
                Id = MaintenanceId,
                BusId = BusId,
                Status = "Concluída"
            });

        var result = await _service.UpdateAsync(
            MaintenanceId,
            CreateValidMaintenance());

        Assert.Null(result);

        _repository.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<string>(),
                It.IsAny<MaintenanceDB>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenTransitionIsInvalid_ReturnsNull()
    {
        SetupExistingBus();

        _repository
            .Setup(repository => repository.GetByIdAsync(MaintenanceId))
            .ReturnsAsync(new MaintenanceDB
            {
                Id = MaintenanceId,
                BusId = BusId,
                ServiceType = "Revisão",
                Description = "Revisão inicial",
                ServiceDate = DateTime.UtcNow.AddDays(-1),
                Status = "Agendada"
            });

        var maintenance = CreateValidMaintenance();
        maintenance.ServiceDate = DateTime.UtcNow.AddDays(-1);
        maintenance.Status = "Concluída";

        var result = await _service.UpdateAsync(
            MaintenanceId,
            maintenance);

        Assert.Null(result);

        _repository.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<string>(),
                It.IsAny<MaintenanceDB>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenIdIsInvalid_ReturnsFalseWithoutRepositoryCall()
    {
        var result = await _service.DeleteAsync(InvalidId);

        Assert.False(result);

        _repository.Verify(
            repository => repository.DeleteAsync(
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryDeletesMaintenance_ReturnsTrue()
    {
        _repository
            .Setup(repository => repository.DeleteAsync(MaintenanceId))
            .ReturnsAsync(true);

        var result = await _service.DeleteAsync(MaintenanceId);

        Assert.True(result);

        _repository.Verify(
            repository => repository.DeleteAsync(MaintenanceId),
            Times.Once);
    }

    private void SetupExistingBus()
    {
        _busRepository
            .Setup(repository => repository.GetBusByIdAsync(BusId))
            .ReturnsAsync(new BusDB
            {
                Id = BusId,
                Number = "001",
                Plate = "ABC1234",
                Line = "Ônibus de teste",
                Capacity = 40
            });
    }

    private static MaintenanceDTOAPI CreateValidMaintenance()
    {
        return new MaintenanceDTOAPI
        {
            BusId = BusId,
            ServiceType = "Revisão",
            Description = "Revisão preventiva",
            ServiceDate = DateTime.UtcNow.AddDays(2),
            Cost = 250m,
            Status = "Agendada"
        };
    }
}
