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

        public BusServiceAPI(IBusRepositoryDB busRepository, IMapper mapper) => (_busRepository, _mapper) = (busRepository, mapper);

        public async Task<IEnumerable<BusDTOAPI>> GetAllBuses()
        {
            var buses = await _busRepository.GetAllBusesAsync();
            return _mapper.Map<IEnumerable<BusDTOAPI>>(buses);
        }

        public async Task<BusDTOAPI> GetBusById(string id)
        {
            var bus = await _busRepository.GetBusByIdAsync(id);
            return _mapper.Map<BusDTOAPI>(bus);
        }

        public async Task<BusDTOAPI> CreateBus(BusDTOAPI busDto)
        {
            var bus = _mapper.Map<BusDB>(busDto);
            var createdBus = await _busRepository.CreateBus(bus);
            return _mapper.Map<BusDTOAPI>(createdBus);
        }

        public async Task<BusDTOAPI> UpdateBus(string id, BusDTOAPI busDto)
        {
            var existingBus = await _busRepository.GetBusByIdAsync(id);
            if (existingBus == null)
            {
                return null; // Ônibus não encontrado
            }

            _mapper.Map(busDto, existingBus);
            await _busRepository.UpdateBusAsync(id, existingBus);

            // Recarregue o ônibus atualizado do banco para retorno
            var updatedBus = await _busRepository.GetBusByIdAsync(id);
            return _mapper.Map<BusDTOAPI>(updatedBus);
        }

        public async Task<bool> DeleteBus(string id)
        {
            var existingBus = await _busRepository.GetBusByIdAsync(id);
            if (existingBus == null)
            {
                return false; // Ônibus não encontrado
            }

            var result = await _busRepository.DeleteBus(existingBus.Id);
            return result;
        }
    }
}
