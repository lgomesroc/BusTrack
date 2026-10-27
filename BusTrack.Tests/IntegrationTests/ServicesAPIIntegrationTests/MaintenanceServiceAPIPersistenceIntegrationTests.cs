using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.MappingsAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.RepositoriesDB;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using Xunit.Sdk;

namespace BusTrack.Tests.IntegrationTests.ServicesAPIIntegrationTests;

/// <summary>
/// Integração real entre MaintenanceServiceAPI, repositórios MongoDB e persistência.
/// Configure BUS_TRACK_TEST_MONGO_CONNECTION_STRING para executar esta suíte.
/// Cada teste usa um banco isolado e o remove ao terminar.
/// </summary>
public sealed class MaintenanceServiceAPIPersistenceIntegrationTests : IAsyncLifetime
{
    private const string ConnectionStringVariable = "BUS_TRACK_TEST_MONGO_CONNECTION_STRING";
    private const string DeleteReason = "Registro de teste removido";

    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly MaintenanceRepositoryDB _maintenanceRepository;
    private readonly MaintenanceAuditRepositoryDB _auditRepository;
    private readonly BusRepositoryDB _busRepository;
    private readonly MaintenanceServiceAPI _service;

    public MaintenanceServiceAPIPersistenceIntegrationTests()
    {
        var connectionString = Environment.GetEnvironmentVariable(ConnectionStringVariable);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new SkipException(
                $"Defina a variável {ConnectionStringVariable} para executar testes de integração com MongoDB real.");
        }

        _client = new MongoClient(connectionString);
        var databaseName = $"BusTrackMaintenanceTests_{Guid.NewGuid():N}";
        _database = _client.GetDatabase(databaseName);

        _maintenanceRepository = new MaintenanceRepositoryDB(_database);
        _auditRepository = new MaintenanceAuditRepositoryDB(_database);
        _busRepository = new BusRepositoryDB(_database);

        var mapperConfiguration = new MapperConfiguration(
            configuration => configuration.AddProfile<MappingProfileAPI>(),
            NullLoggerFactory.Instance);

        _service = new MaintenanceServiceAPI(
            _maintenanceRepository,
            _busRepository,
            mapperConfiguration.CreateMapper(),
            _auditRepository,
            new HttpContextAccessor());
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.DropDatabaseAsync(_database.DatabaseNamespace.DatabaseName);
    }

    [Fact]
    public async Task CreateAsync_PersistsMaintenanceAndCreationAudit()
    {
        var bus = await CreateBusAsync();

        var created = await _service.CreateAsync(CreateMaintenance(bus.Id!));

        Assert.NotNull(created);
        Assert.False(string.IsNullOrWhiteSpace(created.Id));

        var persisted = await _maintenanceRepository.GetByIdAsync(created.Id!);
        Assert.NotNull(persisted);
        Assert.Equal(bus.Id, persisted.BusId);
        Assert.Equal("Revisão", persisted.ServiceType);
        Assert.Equal("Agendada", persisted.Status);
        Assert.False(persisted.IsDeleted);

        var audits = (await _auditRepository.GetByMaintenanceIdAsync(created.Id!)).ToList();
        var audit = Assert.Single(audits);
        Assert.Equal("Created", audit.Action);
        Assert.Equal(created.Id, audit.MaintenanceId);
        Assert.NotNull(audit.Snapshot);
        Assert.NotEmpty(audit.Changes);
    }

    [Fact]
    public async Task UpdateAsync_PersistsChangesAndAuditSnapshot()
    {
        var bus = await CreateBusAsync();
        var created = await _service.CreateAsync(CreateMaintenance(bus.Id!));
        Assert.NotNull(created);

        var update = CreateMaintenance(bus.Id!);
        update.ServiceDate = DateTime.UtcNow.AddHours(-1);
        update.Status = "Em andamento";
        update.Description = "Revisão iniciada no teste";
        update.Cost = 325.50m;

        var updated = await _service.UpdateAsync(created.Id!, update);

        Assert.NotNull(updated);
        Assert.Equal("Em andamento", updated.Status);
        Assert.Equal("Revisão iniciada no teste", updated.Description);
        Assert.Equal(325.50m, updated.Cost);

        var persisted = await _maintenanceRepository.GetByIdAsync(created.Id!);
        Assert.NotNull(persisted);
        Assert.Equal("Em andamento", persisted.Status);
        Assert.Equal("Revisão iniciada no teste", persisted.Description);
        Assert.Equal(325.50m, persisted.Cost);

        var audits = (await _auditRepository.GetByMaintenanceIdAsync(created.Id!)).ToList();
        Assert.Equal(2, audits.Count);
        var updateAudit = Assert.Single(audits.Where(audit => audit.Action == "Updated"));
        Assert.NotNull(updateAudit.Snapshot);
        Assert.Contains(updateAudit.Changes, change => change.Field == "Description");
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletesMaintenanceAndPersistsReasonInAudit()
    {
        var bus = await CreateBusAsync();
        var created = await _service.CreateAsync(CreateMaintenance(bus.Id!));
        Assert.NotNull(created);

        var deleted = await _service.DeleteAsync(created.Id!, DeleteReason);

        Assert.True(deleted);
        Assert.Null(await _maintenanceRepository.GetByIdAsync(created.Id!));
        Assert.Empty(await _maintenanceRepository.GetAllAsync());

        var includingDeleted = await _maintenanceRepository.GetByIdIncludingDeletedAsync(created.Id!);
        Assert.NotNull(includingDeleted);
        Assert.True(includingDeleted.IsDeleted);
        Assert.Equal(DeleteReason, includingDeleted.DeletionReason);
        Assert.NotNull(includingDeleted.DeletedAtUtc);

        var audits = (await _auditRepository.GetByMaintenanceIdAsync(created.Id!)).ToList();
        Assert.Equal(2, audits.Count);
        var deleteAudit = Assert.Single(audits.Where(audit => audit.Action == "Deleted"));
        Assert.Equal(DeleteReason, deleteAudit.Reason);
        Assert.NotNull(deleteAudit.Snapshot);
    }

    [Fact]
    public async Task CreateAsync_WhenBusDoesNotExist_DoesNotPersistMaintenanceOrAudit()
    {
        var result = await _service.CreateAsync(
            CreateMaintenance("507f1f77bcf86cd799439099"));

        Assert.Null(result);
        Assert.Empty(await _maintenanceRepository.GetAllAsync());
        Assert.Empty(await _database
            .GetCollection<MaintenanceAuditDB>("MaintenanceAudits")
            .Find(_ => true)
            .ToListAsync());
    }

    [Fact]
    public async Task GetByIdAsync_WhenIdIsInvalid_ReturnsNullWithoutDatabaseRead()
    {
        var result = await _service.GetByIdAsync("id-invalido");

        Assert.Null(result);
    }

    private async Task<BusDB> CreateBusAsync()
    {
        return await _busRepository.CreateBus(new BusDB
        {
            Number = "TEST-001",
            Plate = "TST1234",
            Line = "Linha de teste",
            Capacity = 40
        });
    }

    private static MaintenanceDTOAPI CreateMaintenance(string busId)
    {
        return new MaintenanceDTOAPI
        {
            BusId = busId,
            ServiceType = "Revisão",
            Description = "Revisão preventiva de integração",
            ServiceDate = DateTime.UtcNow.AddDays(2),
            Cost = 250m,
            Status = "Agendada"
        };
    }
}
