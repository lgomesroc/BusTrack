using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Bson;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class MaintenanceServiceAPI : IMaintenanceServiceAPI
    {
        private static readonly string[] AllowedStatuses =
            ["Agendada", "Em andamento", "Concluída", "Cancelada"];

        private readonly IMaintenanceRepositoryDB _repository;
        private readonly IBusRepositoryDB _busRepository;
        private readonly IMapper _mapper;

        public MaintenanceServiceAPI(
            IMaintenanceRepositoryDB repository,
            IBusRepositoryDB busRepository,
            IMapper mapper)
        {
            _repository = repository;
            _busRepository = busRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MaintenanceDTOAPI>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<MaintenanceDTOAPI>>(items);
        }

        public async Task<MaintenanceDTOAPI?> GetByIdAsync(string id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item is null ? null : _mapper.Map<MaintenanceDTOAPI>(item);
        }

        public async Task<MaintenanceDTOAPI?> CreateAsync(MaintenanceDTOAPI maintenance)
        {
            var normalized = await NormalizeAndValidateAsync(maintenance);
            if (normalized is null)
            {
                return null;
            }

            var entity = _mapper.Map<MaintenanceDB>(normalized);
            var created = await _repository.CreateAsync(entity);
            return _mapper.Map<MaintenanceDTOAPI>(created);
        }

        public async Task<MaintenanceDTOAPI?> UpdateAsync(string id, MaintenanceDTOAPI maintenance)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
            {
                return null;
            }

            var normalized = await NormalizeAndValidateAsync(maintenance);
            if (normalized is null)
            {
                return null;
            }

            _mapper.Map(normalized, existing);
            existing.Id = id;

            if (!await _repository.UpdateAsync(id, existing))
            {
                return null;
            }

            return _mapper.Map<MaintenanceDTOAPI>(existing);
        }

        public Task<bool> DeleteAsync(string id) => _repository.DeleteAsync(id);

        private async Task<MaintenanceDTOAPI?> NormalizeAndValidateAsync(
            MaintenanceDTOAPI maintenance)
        {
            if (string.IsNullOrWhiteSpace(maintenance.BusId) ||
                string.IsNullOrWhiteSpace(maintenance.ServiceType) ||
                string.IsNullOrWhiteSpace(maintenance.Description) ||
                maintenance.ServiceDate == default ||
                maintenance.Cost < 0 ||
                !AllowedStatuses.Contains(maintenance.Status, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }

            if (!ObjectId.TryParse(maintenance.BusId, out _))
            {
                return null;
            }

            var bus = await _busRepository.GetBusByIdAsync(maintenance.BusId);
            if (bus is null)
            {
                return null;
            }

            maintenance.BusId = bus.Id!;
            maintenance.BusNumber = bus.Number ?? bus.Plate ?? bus.Id!;
            maintenance.ServiceType = maintenance.ServiceType.Trim();
            maintenance.Description = maintenance.Description.Trim();
            maintenance.Status = AllowedStatuses.First(
                status => status.Equals(maintenance.Status.Trim(), StringComparison.OrdinalIgnoreCase));

            if (maintenance.Status == "Concluída" && maintenance.CompletedDate is null)
            {
                maintenance.CompletedDate = DateTime.UtcNow;
            }

            if (maintenance.Status != "Concluída")
            {
                maintenance.CompletedDate = null;
            }

            return maintenance;
        }
    }
}