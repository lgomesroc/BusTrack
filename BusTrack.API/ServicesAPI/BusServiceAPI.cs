using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class BusServiceAPI : IBusServiceAPI
    {
        private readonly IBusRepositoryDB _busRepository;
        private readonly IMapper _mapper;

        public BusServiceAPI(IBusRepositoryDB busRepository, IMapper mapper)
        {
            _busRepository = busRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BusDTOAPI>> GetAllBuses()
        {
            var buses = await _busRepository.GetAllBuses();
            return _mapper.Map<IEnumerable<BusDTOAPI>>(buses);
        }

        public async Task<BusDTOAPI> GetBusById(int id)
        {
            var bus = await _busRepository.GetBusById(id);
            return _mapper.Map<BusDTOAPI>(bus);
        }

        public async Task<BusDTOAPI> CreateBus(BusDTOAPI busDto)
        {
            var bus = _mapper.Map<BusDB>(busDto);
            var createdBus = await _busRepository.CreateBus(bus);
            return _mapper.Map<BusDTOAPI>(createdBus);
        }

        public async Task<BusDTOAPI> UpdateBus(int id, BusDTOAPI busDto)
        {
            var existingBus = await _busRepository.GetBusById(id);
            if (existingBus == null)
            {
                return null; // Ônibus não encontrado
            }

            _mapper.Map(busDto, existingBus);
            var updatedBus = await _busRepository.UpdateBus(existingBus);
            return _mapper.Map<BusDTOAPI>(updatedBus);
        }

        public async Task<bool> DeleteBus(int id)
        {
            var existingBus = await _busRepository.GetBusById(id);
            if (existingBus == null)
            {
                return false; // Ônibus não encontrado
            }

            var result = await _busRepository.DeleteBus(existingBus);
            return result;
        }
    }
}
