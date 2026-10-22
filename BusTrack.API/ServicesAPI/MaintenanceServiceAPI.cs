
using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using MongoDB.Bson;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class MaintenanceServiceAPI : IMaintenanceServiceAPI
    {
        private static readonly string[] AllowedStatuses =
        [
            "Agendada",
            "Em andamento",
            "Concluída",
            "Cancelada"
        ];

        private const int MaxServiceTypeLength = 80;
        private const int MaxDescriptionLength = 1000;
        private const decimal MaxCost = 999999999m;

        private readonly IMaintenanceRepositoryDB _repository;
        private readonly IBusRepositoryDB _busRepository;
        private readonly IMaintenanceAuditRepositoryDB? _auditRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        // Mantém compatibilidade com testes e chamadas existentes
        // que ainda instanciam o serviço com três dependências.
        public MaintenanceServiceAPI(
            IMaintenanceRepositoryDB repository,
            IBusRepositoryDB busRepository,
            IMapper mapper)
        {
            _repository = repository;
            _busRepository = busRepository;
            _mapper = mapper;
        }

        // Construtor utilizado pela injeção de dependências da aplicação.
        public MaintenanceServiceAPI(
            IMaintenanceRepositoryDB repository,
            IBusRepositoryDB busRepository,
            IMapper mapper,
            IMaintenanceAuditRepositoryDB auditRepository,
            IHttpContextAccessor httpContextAccessor)
            : this(repository, busRepository, mapper)
        {
            _auditRepository = auditRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<MaintenanceDTOAPI>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<MaintenanceDTOAPI>>(items);
        }

        public async Task<MaintenanceDTOAPI?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            var item = await _repository.GetByIdAsync(id);

            return item is null
                ? null
                : _mapper.Map<MaintenanceDTOAPI>(item);
        }

        public async Task<IEnumerable<MaintenanceAuditDB>> GetAuditAsync(
            string id)
        {
            if (!ObjectId.TryParse(id, out _) ||
                _auditRepository is null)
            {
                return [];
            }

            var item = await _repository.GetByIdIncludingDeletedAsync(id);

            if (item is null)
            {
                return [];
            }

            return await _auditRepository.GetByMaintenanceIdAsync(id);
        }

        public async Task<MaintenanceDTOAPI?> CreateAsync(
            MaintenanceDTOAPI maintenance)
        {
            var normalized = await NormalizeAndValidateAsync(maintenance);

            if (normalized is null)
            {
                return null;
            }

            var entity = _mapper.Map<MaintenanceDB>(normalized);
            entity.IsDeleted = false;
            entity.DeletedAtUtc = null;
            entity.DeletedBy = null;
            entity.DeletionReason = null;

            var created = await _repository.CreateAsync(entity);

            await RecordAuditAsync(
                created,
                "Created",
                "Criação da manutenção",
                null,
                created);

            return _mapper.Map<MaintenanceDTOAPI>(created);
        }

        public async Task<MaintenanceDTOAPI?> UpdateAsync(
            string id,
            MaintenanceDTOAPI maintenance)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }

            var existing = await _repository.GetByIdAsync(id);

            if (existing is null || IsTerminalStatus(existing.Status))
            {
                return null;
            }

            var normalized = await NormalizeAndValidateAsync(maintenance);

            if (normalized is null ||
                !IsAllowedTransition(existing.Status, normalized.Status))
            {
                return null;
            }

            var before = existing.ToBsonDocument();

            _mapper.Map(normalized, existing);
            existing.Id = id;

            var updated = await _repository.UpdateAsync(id, existing);

            if (!updated)
            {
                return null;
            }

            var saved = await _repository.GetByIdAsync(id);

            if (saved is null)
            {
                return null;
            }

            await RecordAuditAsync(
                saved,
                "Updated",
                "Alteração da manutenção",
                before,
                saved);

            return _mapper.Map<MaintenanceDTOAPI>(saved);
        }

        public async Task<bool> DeleteAsync(string id, string reason)
        {
            if (!ObjectId.TryParse(id, out _) ||
                string.IsNullOrWhiteSpace(reason) ||
                reason.Trim().Length > 1000)
            {
                return false;
            }

            var existing = await _repository.GetByIdAsync(id);

            if (existing is null ||
                !string.Equals(
                    existing.Status?.Trim(),
                    "Agendada",
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var before = existing.ToBsonDocument();

            existing.IsDeleted = true;
            existing.DeletedAtUtc = DateTime.UtcNow;
            existing.DeletedBy = GetCurrentActor();
            existing.DeletionReason = reason.Trim();

            var deleted = await _repository.SoftDeleteAsync(id, existing);

            if (!deleted)
            {
                return false;
            }

            await RecordAuditAsync(
                existing,
                "Deleted",
                reason.Trim(),
                before,
                existing);

            return true;
        }

        private async Task RecordAuditAsync(
            MaintenanceDB maintenance,
            string action,
            string? reason,
            BsonDocument? before,
            MaintenanceDB after)
        {
            // Em produção, o repositório é registrado no Program.cs.
            // Se estiver ausente, não fingimos que a auditoria foi gravada.
            if (_auditRepository is null)
            {
                throw new InvalidOperationException(
                    "O repositório de auditoria de manutenção não foi configurado.");
            }

            var afterDocument = after.ToBsonDocument();

            var audit = new MaintenanceAuditDB
            {
                MaintenanceId = maintenance.Id!,
                Action = action,
                OccurredAtUtc = DateTime.UtcNow,
                PerformedBy = GetCurrentActor(),
                Reason = reason,
                Snapshot = afterDocument,
                Changes = BuildChanges(before, afterDocument)
            };

            await _auditRepository.CreateAsync(audit);
        }

        private static List<MaintenanceAuditChangeDB> BuildChanges(
            BsonDocument? before,
            BsonDocument after)
        {
            var changes = new List<MaintenanceAuditChangeDB>();

            if (before is null)
            {
                foreach (var field in after.Elements)
                {
                    changes.Add(new MaintenanceAuditChangeDB
                    {
                        Field = field.Name,
                        OldValue = null,
                        NewValue = field.Value.ToJson()
                    });
                }

                return changes;
            }

            var names = before.Names
                .Union(after.Names, StringComparer.Ordinal)
                .Distinct(StringComparer.Ordinal);

            foreach (var name in names)
            {
                var hasOld = before.TryGetValue(name, out var oldValue);
                var hasNew = after.TryGetValue(name, out var newValue);

                var oldText = hasOld ? oldValue.ToJson() : null;
                var newText = hasNew ? newValue.ToJson() : null;

                if (string.Equals(oldText, newText, StringComparison.Ordinal))
                {
                    continue;
                }

                changes.Add(new MaintenanceAuditChangeDB
                {
                    Field = name,
                    OldValue = oldText,
                    NewValue = newText
                });
            }

            return changes;
        }

        private string GetCurrentActor()
        {
            var user = _httpContextAccessor?.HttpContext?.User;

            var actor =
                user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                user?.FindFirst("sub")?.Value ??
                user?.FindFirst(ClaimTypes.Email)?.Value ??
                user?.Identity?.Name;

            return string.IsNullOrWhiteSpace(actor)
                ? "system"
                : actor;
        }

        private async Task<MaintenanceDTOAPI?> NormalizeAndValidateAsync(
            MaintenanceDTOAPI maintenance)
        {
            if (maintenance is null)
            {
                return null;
            }

            var busId = maintenance.BusId?.Trim();
            var serviceType = maintenance.ServiceType?.Trim();
            var description = maintenance.Description?.Trim();
            var statusInput = maintenance.Status?.Trim();

            if (string.IsNullOrWhiteSpace(busId) ||
                string.IsNullOrWhiteSpace(serviceType) ||
                string.IsNullOrWhiteSpace(description) ||
                string.IsNullOrWhiteSpace(statusInput))
            {
                return null;
            }

            if (serviceType.Length > MaxServiceTypeLength ||
                description.Length > MaxDescriptionLength)
            {
                return null;
            }

            if (maintenance.ServiceDate == default ||
                maintenance.ServiceDate == DateTime.MinValue ||
                maintenance.ServiceDate == DateTime.MaxValue)
            {
                return null;
            }

            if (maintenance.Cost < 0 ||
                maintenance.Cost > MaxCost ||
                decimal.Round(maintenance.Cost, 2) != maintenance.Cost)
            {
                return null;
            }

            var normalizedStatus = AllowedStatuses.FirstOrDefault(
                status => status.Equals(
                    statusInput,
                    StringComparison.OrdinalIgnoreCase));

            if (normalizedStatus is null)
            {
                return null;
            }

            var serviceDate = GetBrazilDate(maintenance.ServiceDate);
            var today = GetBrazilToday();

            // Uma manutenção agendada não pode ter data passada.
            if (normalizedStatus == "Agendada" && serviceDate < today)
            {
                return null;
            }

            // Serviços em andamento ou concluídos não podem ter
            // data de serviço futura.
            if (normalizedStatus is "Em andamento" or "Concluída" &&
                serviceDate > today)
            {
                return null;
            }

            if (!ObjectId.TryParse(busId, out _))
            {
                return null;
            }

            var bus = await _busRepository.GetBusByIdAsync(busId);

            if (bus is null || string.IsNullOrWhiteSpace(bus.Id))
            {
                return null;
            }

            var completedDate = maintenance.CompletedDate;

            if (completedDate.HasValue)
            {
                var completed = completedDate.Value;

                if (completed == DateTime.MinValue ||
                    completed == DateTime.MaxValue)
                {
                    return null;
                }

                var completedUtc = AsUtc(completed);

                // A conclusão não pode ocorrer no futuro.
                if (completedUtc > DateTime.UtcNow)
                {
                    return null;
                }

                // A conclusão não pode anteceder o dia do serviço.
                if (GetBrazilDate(completed) < serviceDate)
                {
                    return null;
                }
            }

            if (normalizedStatus == "Concluída")
            {
                completedDate ??= DateTime.UtcNow;

                if (AsUtc(completedDate.Value) > DateTime.UtcNow ||
                    GetBrazilDate(completedDate.Value) < serviceDate)
                {
                    return null;
                }
            }
            else
            {
                // Somente manutenções concluídas mantêm data de conclusão.
                completedDate = null;
            }

            maintenance.BusId = bus.Id;
            maintenance.BusNumber = !string.IsNullOrWhiteSpace(bus.Number)
                ? bus.Number.Trim()
                : !string.IsNullOrWhiteSpace(bus.Plate)
                    ? bus.Plate.Trim()
                    : bus.Id;

            maintenance.ServiceType = serviceType;
            maintenance.Description = description;
            maintenance.Status = normalizedStatus;
            maintenance.ServiceDate = AsUtc(maintenance.ServiceDate);
            maintenance.CompletedDate = completedDate.HasValue
                ? AsUtc(completedDate.Value)
                : null;

            return maintenance;
        }

        private static bool IsTerminalStatus(string? status)
        {
            return string.Equals(
                       status?.Trim(),
                       "Concluída",
                       StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(
                       status?.Trim(),
                       "Cancelada",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsAllowedTransition(
            string? currentStatus,
            string? newStatus)
        {
            if (string.IsNullOrWhiteSpace(currentStatus) ||
                string.IsNullOrWhiteSpace(newStatus))
            {
                return false;
            }

            return currentStatus.Trim() switch
            {
                "Agendada" =>
                    newStatus is "Agendada" or "Em andamento" or "Cancelada",

                "Em andamento" =>
                    newStatus is "Em andamento" or "Concluída" or "Cancelada",

                _ => false
            };
        }

        private static DateTime AsUtc(DateTime value)
        {
            return value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };
        }

        private static DateTime GetBrazilToday()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                GetBrazilTimeZone()).Date;
        }

        private static DateTime GetBrazilDate(DateTime value)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(
                AsUtc(value),
                GetBrazilTimeZone()).Date;
        }

        private static TimeZoneInfo GetBrazilTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById(
                    "E. South America Standard Time");
            }
        }
    }
}
