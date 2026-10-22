
using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.MappingsAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BusTrack.Tests.UnitTests.ServicesAPI;

public class MaintenanceServiceAPIUnitTests
{
    private const string MaintenanceId = "507f1f77bcf86cd799439011";
    private const string BusId = "507f1f77bcf86cd799439012";
    private const string InvalidId = "id-invalido";
    private const string DeleteReason = "Registro duplicado";

    private readonly Mock<IMaintenanceRepositoryDB> _repository = new();
    private readonly Mock<IBusRepositoryDB> _busRepository = new();
    private readonly Mock<IMaintenanceAuditRepositoryDB> _auditRepository = new();
    private readonly MaintenanceServiceAPI _service;

    public MaintenanceServiceAPIUnitTests()
    {
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<MappingProfileAPI>(),
            NullLoggerFactory.Instance);

        _auditRepository
            .Setup(repository => repository.CreateAsync(
                It.IsAny<MaintenanceAuditDB>()))
            .ReturnsAsync((MaintenanceAuditDB audit) => audit);

        _service = new MaintenanceServiceAPI(
            _repository.Object,
            _busRepository.Object,
            configuration.CreateMapper(),
            _auditRepository.Object,
            new HttpContextAccessor());
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
    }

    [Fact]
    public async Task GetAllAsync_WhenMaintenancesExist_ReturnsMappedItems()
    {
        _repository
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(new List<MaintenanceDB>
            {
                CreateEntity()
            });

        var result = (await _service.GetAllAsync()).ToList();

        var maintenance = Assert.Single(result);

        Assert.Equal(MaintenanceId, maintenance.Id);
        Assert.Equal(BusId, maintenance.BusId);
        Assert.Equal("Revisão", maintenance.ServiceType);
    }

    [Fact]
    public async Task GetByIdAsync_WhenIdIsInvalid_ReturnsNull()
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
            .ReturnsAsync(CreateEntity());

        var result = await _service.GetByIdAsync(MaintenanceId);

        Assert.NotNull(result);
        Assert.Equal(MaintenanceId, result.Id);
        Assert.Equal("Revisão", result.ServiceType);
    }

    [Fact]
    public async Task CreateAsync_WhenDataIsValid_CreatesAndAuditsMaintenance()
    {
        SetupExistingBus();

        _repository
            .Setup(repository => repository.CreateAsync(
                It.IsAny<MaintenanceDB>()))
            .ReturnsAsync((MaintenanceDB item) =>
            {
                item.Id = MaintenanceId;
                return item;
            });

        var result = await _service.CreateAsync(CreateValidMaintenance());

        Assert.NotNull(result);
        Assert.Equal(MaintenanceId, result.Id);

        _repository.Verify(
            repository => repository.CreateAsync(
                It.IsAny<MaintenanceDB>()),
            Times.Once);

        _auditRepository.Verify(
            repository => repository.CreateAsync(
                It.Is<MaintenanceAuditDB>(audit =>
                    audit.MaintenanceId == MaintenanceId &&
                    audit.Action == "Created")),
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
    public async Task CreateAsync_WhenCostHasTooManyDecimalPlaces_ReturnsNull()
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
            .ReturnsAsync((MaintenanceDB item) =>
            {
                item.Id = MaintenanceId;
                return item;
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
            repository => repository.GetByIdAsync(It.IsAny<string>()),
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

    [Theory]
    [InlineData("Concluída")]
    [InlineData("Cancelada")]
    public async Task UpdateAsync_WhenMaintenanceIsTerminal_ReturnsNull(
        string status)
    {
        var existing = CreateEntity();
        existing.Status = status;

        _repository
            .Setup(repository => repository.GetByIdAsync(MaintenanceId))
            .ReturnsAsync(existing);

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
    public async Task UpdateAsync_WhenTransitionIsValid_UpdatesAndAudits()
    {
        SetupExistingBus();

        var existing = CreateEntity();
        existing.ServiceDate = DateTime.UtcNow.AddDays(-1);
        existing.Status = "Agendada";

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

        _auditRepository.Verify(
            repository => repository.CreateAsync(
                It.Is<MaintenanceAuditDB>(audit =>
                    audit.MaintenanceId == MaintenanceId &&
                    audit.Action == "Updated" &&
                    audit.Changes.Count > 0)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenTransitionIsInvalid_ReturnsNull()
    {
        var existing = CreateEntity();
        existing.ServiceDate = DateTime.UtcNow.AddDays(-1);
        existing.Status = "Agendada";

        _repository
            .Setup(repository => repository.GetByIdAsync(MaintenanceId))
            .ReturnsAsync(existing);

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
    public async Task DeleteAsync_WhenIdIsInvalid_ReturnsFalse()
    {
        var result = await _service.DeleteAsync(InvalidId, DeleteReason);

        Assert.False(result);

        _repository.Verify(
            repository => repository.SoftDeleteAsync(
                It.IsAny<string>(),
                It.IsAny<MaintenanceDB>()),
            Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task DeleteAsync_WhenReasonIsBlank_ReturnsFalse(
        string reason)
    {
        var result = await _service.DeleteAsync(MaintenanceId, reason);

        Assert.False(result);

        _repository.Verify(
            repository => repository.SoftDeleteAsync(
                It.IsAny<string>(),
                It.IsAny<MaintenanceDB>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenMaintenanceDoesNotExist_ReturnsFalse()
    {
        _repository
            .Setup(repository => repository.GetByIdAsync(MaintenanceId))
            .ReturnsAsync((MaintenanceDB?)null);

        var result = await _service.DeleteAsync(
            MaintenanceId,
            DeleteReason);

        Assert.False(result);
    }

    [Theory]
    [InlineData("Em andamento")]
    [InlineData("Concluída")]
    [InlineData("Cancelada")]
    public async Task DeleteAsync_WhenStatusDoesNotAllowDeletion_ReturnsFalse(
        string status)
    {
        var existing = CreateEntity();
        existing.Status = status;

        _repository
            .Setup(repository => repository.GetByIdAsync(MaintenanceId))
            .ReturnsAsync(existing);

        var result = await _service.DeleteAsync(
            MaintenanceId,
            DeleteReason);

        Assert.False(result);

        _repository.Verify(
            repository => repository.SoftDeleteAsync(
                It.IsAny<string>(),
                It.IsAny<MaintenanceDB>()),
            Times.Never);

        _auditRepository.Verify(
            repository => repository.CreateAsync(
                It.IsAny<MaintenanceAuditDB>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenScheduled_SoftDeletesAndAudits()
    {
        var existing = CreateEntity();
        existing.Status = "Agendada";

        _repository
            .Setup(repository => repository.GetByIdAsync(MaintenanceId))
            .ReturnsAsync(existing);

        _repository
            .Setup(repository => repository.SoftDeleteAsync(
                MaintenanceId,
                It.IsAny<MaintenanceDB>()))
            .ReturnsAsync(true);

        var result = await _service.DeleteAsync(
            MaintenanceId,
            DeleteReason);

        Assert.True(result);

        _repository.Verify(
            repository => repository.SoftDeleteAsync(
                MaintenanceId,
                It.Is<MaintenanceDB>(item =>
                    item.IsDeleted &&
                    item.DeletionReason == DeleteReason &&
                    item.DeletedAtUtc.HasValue)),
            Times.Once);

        _auditRepository.Verify(
            repository => repository.CreateAsync(
                It.Is<MaintenanceAuditDB>(audit =>
                    audit.MaintenanceId == MaintenanceId &&
                    audit.Action == "Deleted" &&
                    audit.Reason == DeleteReason)),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryRejectsSoftDelete_ReturnsFalse()
    {
        var existing = CreateEntity();
        existing.Status = "Agendada";

        _repository
            .Setup(repository => repository.GetByIdAsync(MaintenanceId))
            .ReturnsAsync(existing);

        _repository
            .Setup(repository => repository.SoftDeleteAsync(
                MaintenanceId,
                It.IsAny<MaintenanceDB>()))
            .ReturnsAsync(false);

        var result = await _service.DeleteAsync(
            MaintenanceId,
            DeleteReason);

        Assert.False(result);

        _auditRepository.Verify(
            repository => repository.CreateAsync(
                It.IsAny<MaintenanceAuditDB>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAuditAsync_WhenIdIsInvalid_ReturnsEmptyCollection()
    {
        var result = await _service.GetAuditAsync(InvalidId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAuditAsync_WhenMaintenanceExists_ReturnsHistory()
    {
        _repository
            .Setup(repository =>
                repository.GetByIdIncludingDeletedAsync(MaintenanceId))
            .ReturnsAsync(CreateEntity());

        var audits = new List<MaintenanceAuditDB>
        {
            new()
            {
                MaintenanceId = MaintenanceId,
                Action = "Created",
                PerformedBy = "test-user",
                OccurredAtUtc = DateTime.UtcNow
            }
        };

        _auditRepository
            .Setup(repository =>
                repository.GetByMaintenanceIdAsync(MaintenanceId))
            .ReturnsAsync(audits);

        var result = await _service.GetAuditAsync(MaintenanceId);

        Assert.NotNull(result);
        Assert.Single(result);
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

    private static MaintenanceDB CreateEntity()
    {
        return new MaintenanceDB
        {
            Id = MaintenanceId,
            BusId = BusId,
            BusNumber = "001",
            ServiceType = "Revisão",
            Description = "Revisão preventiva",
            ServiceDate = DateTime.UtcNow.AddDays(2),
            Cost = 250m,
            Status = "Agendada"
        };
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
