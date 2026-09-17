using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class BusServiceAPI : IBusServiceAPI
    {
        private readonly IBusRepositoryDB _busRepository;
        private readonly IMapper _mapper;

        public BusServiceAPI(
            IBusRepositoryDB busRepository,
            IMapper mapper)
        {
            _busRepository = busRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BusDTOAPI>>
            GetAllBuses()
        {
            var buses =
                await _busRepository
                    .GetAllBusesAsync();

            return _mapper.Map<
                IEnumerable<BusDTOAPI>>(
                buses);
        }

        public List<BusDB> GetBuses()
        {
            var buses =
                _busRepository
                    .GetAllBusesAsync()
                    .Result;

            return buses.ToList();
        }

        public async Task<BusDTOAPI?>
            GetBusById(
                string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var bus =
                await _busRepository
                    .GetBusByIdAsync(id);

            if (bus == null)
            {
                return null;
            }

            return _mapper.Map<BusDTOAPI>(
                bus);
        }

        public async Task<BusDTOAPI>
            CreateBus(
                BusDTOAPI busDto)
        {
            var bus =
                _mapper.Map<BusDB>(
                    busDto);

            var createdBus =
                await _busRepository
                    .CreateBus(bus);

            return _mapper.Map<BusDTOAPI>(
                createdBus);
        }

        public async Task<BusDTOAPI?>
            UpdateBus(
                string? id,
                BusDTOAPI busDto)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var existingBus =
                await _busRepository
                    .GetBusByIdAsync(id);

            if (existingBus == null)
            {
                return null;
            }

            _mapper.Map(
                busDto,
                existingBus);

            await _busRepository
                .UpdateBusAsync(
                    id,
                    existingBus);

            var updatedBus =
                await _busRepository
                    .GetBusByIdAsync(id);

            if (updatedBus == null)
            {
                return null;
            }

            return _mapper.Map<BusDTOAPI>(
                updatedBus);
        }

        public async Task<bool>
            DeleteBus(
                string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            var existingBus =
                await _busRepository
                    .GetBusByIdAsync(id);

            if (existingBus == null)
            {
                return false;
            }

            return await _busRepository
                .DeleteBus(id);
        }
    }
}
