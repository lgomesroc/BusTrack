using AutoMapper;
using BusTrack.BusTrack.API.DTOModelAPI;
using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using BusTrack.BusTrack.API.ModelsAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class DriverServiceAPI : IDriverServiceAPI
    {
        private readonly IDriverRepositoryDB _driverRepository;
        private readonly IMapper _mapper;

        public DriverServiceAPI(IDriverRepositoryDB driverRepository, IMapper mapper)
        {
            _driverRepository = driverRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DriverDTOAPI>> GetAllDrivers()
        {
            var drivers = await _driverRepository.GetAllDrivers();
            return _mapper.Map<IEnumerable<DriverDTOAPI>>(drivers);
        }

        public async Task<DriverDTOAPI> GetDriverById(int id)
        {
            var driver = await _driverRepository.GetDriverById(id);
            return _mapper.Map<DriverDTOAPI>(driver);
        }

        public async Task<DriverDTOAPI> CreateDriver(DriverDTOAPI driver)
        {
            var driverModel = _mapper.Map<DriverModelAPI>(driver);
            var createdDriver = await _driverRepository.CreateDriver(driverModel);
            return _mapper.Map<DriverDTOAPI>(createdDriver);
        }

        public async Task<DriverDTOAPI> UpdateDriver(int id, DriverDTOAPI driver)
        {
            var driverModel = _mapper.Map<DriverModelAPI>(driver);
            var updatedDriver = await _driverRepository.UpdateDriver(id, driverModel);
            return _mapper.Map<v>(updatedDriver);
        }

        public async Task<bool> DeleteDriver(int id)
        {
            return await _driverRepository.DeleteDriver(id);
        }
    }
}
