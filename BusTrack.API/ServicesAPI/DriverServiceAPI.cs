using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using BusTrack.BusTrack.API.ModelsAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.BusTrack.Updater.Drivers;
using MongoDB.Driver;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class DriverServiceAPI : IDriverServiceAPI
    {
        private readonly IMongoCollection<DriverDB> _driversCollection;
        private readonly IDriverRepositoryDB _driverRepository;
        private readonly IMapper _mapper;

        public DriverServiceAPI(IMongoClient mongoClient, IDriverRepositoryDB driverRepository, IMapper mapper)
        {
            var database = mongoClient.GetDatabase("YourDatabaseName");
            _driversCollection = database.GetCollection<DriverDB>("YourCollectionName");
            _driverRepository = driverRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DriverDTOAPI>> GetAllDrivers()
        {
            var drivers = await _driverRepository.GetAllDriversAsync();
            return _mapper.Map<IEnumerable<DriverDTOAPI>>(drivers);

        }

        public List<DriverDB> GetDrivers()
        {
            var drivers = _driverRepository.GetAllDriversAsync().Result;
            return drivers.ToList();
        }

        public async Task<DriverDTOAPI> GetDriverById(string id)
        {
            var driver = await _driverRepository.GetDriverByIdAsync(id);
            if (driver == null)
            {
                // Tratar o caso de passageiro não encontrado
                return null;
            }
            return _mapper.Map<DriverDTOAPI>(driver);
        }

        public async Task<DriverDTOAPI> CreateDriver(DriverDTOAPI driver)
        {
            var driverModel = _mapper.Map<DriverModelAPI>(driver);

            // Converter PassengerModelAPI para PassengerDB antes de chamar AddPassengerAsync
            var driverDB = _mapper.Map<DriverDB>(driverModel);

            await _driverRepository.AddDriverAsync(driverDB);

            // Converter de volta para PassengerModelAPI para retorno
            return _mapper.Map<DriverDTOAPI>(driverDB);
        }


        public async Task<DriverDTOAPI> UpdateDriver(string id, DriverDTOAPI driver)
        {
            var driverDB = _mapper.Map<DriverDB>(driver);

            await _driverRepository.UpdateDriverAsync(id, driverDB);

            // Recarregue o motorista atualizado do banco para retorno (opcional)
            var updatedDriver = await _driverRepository.GetDriverByIdAsync(id);
            return _mapper.Map<DriverDTOAPI>(updatedDriver);
        }

        public async Task UpdateDriverAsync(string id, DriverDB driver)
        {
            var filter = Builders<DriverDB>.Filter.Eq("Id", id);

            // Crie uma classe de atualização específica para o nome
            var nameUpdate = new DriverNameUpdater { Name = driver.Name };

            var update = Builders<DriverDB>.Update.Set(p => p.Name, nameUpdate.Name);

            var updateResult = await _driversCollection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                throw new Exception($"Driver with ID {id} not found or not updated.");
            }
        }

        public async Task<bool> DeleteDriver(string id)
        {
            return await _driverRepository.DeleteDriver(id);
        }
    }
}
