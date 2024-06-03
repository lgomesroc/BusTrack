using AutoMapper;
using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ModelsAPI;
using BusTrack.BusTrack.DB.Classes;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using MongoDB.Driver;

namespace BusTrack.BusTrack.API.ServicesAPI
{
    public class PassengerServiceAPI : IPassengerServiceAPI
    {
        private readonly IMongoCollection<PassengerDB> _passengersCollection;
        private readonly IPassengerRepositoryDB _passengerRepository;
        private readonly IMapper _mapper;

        public PassengerServiceAPI(IMongoClient mongoClient, IPassengerRepositoryDB passengerRepository, IMapper mapper)
        {
            var database = mongoClient.GetDatabase("YourDatabaseName");
            _passengersCollection = database.GetCollection<PassengerDB>("YourCollectionName");
            _passengerRepository = passengerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PassengerDTOAPI>> GetAllPassengers()
        {
            var passengers = await _passengerRepository.GetAllPassengersAsync();
            return _mapper.Map<IEnumerable<PassengerDTOAPI>>(passengers);
        }

        public async Task<PassengerDTOAPI> GetPassengerById(string id)
        {
            var passenger = await _passengerRepository.GetPassengerByIdAsync(id);
            if (passenger == null)
            {
                return null;
            }
            return _mapper.Map<PassengerDTOAPI>(passenger);
        }

        public async Task<PassengerDTOAPI> CreatePassenger(PassengerDTOAPI passenger)
        {
            var passengerModel = _mapper.Map<PassengerModelAPI>(passenger);

            var passengerDB = _mapper.Map<PassengerDB>(passengerModel);

            await _passengerRepository.AddPassengerAsync(passengerDB);

            return _mapper.Map<PassengerDTOAPI>(passengerDB);
        }

        public async Task<PassengerDTOAPI> UpdatePassenger(string id, PassengerDTOAPI passenger)
        {
            var passengerDB = _mapper.Map<PassengerDB>(passenger);

            await _passengerRepository.UpdatePassengerAsync(id, passengerDB);

            var updatedPassenger = await _passengerRepository.GetPassengerByIdAsync(id);
            return _mapper.Map<PassengerDTOAPI>(updatedPassenger);
        }

        public async Task<bool> DeletePassenger(string id)
        {
            return await _passengerRepository.DeletePassenger(id);
        }

        public async Task<List<PassengerDB>> GetPassengers()
        {
            var passengers = (await _passengerRepository.GetAllPassengersAsync()).ToList();
            return passengers;
        }

        public async Task AddPassengerAsync(PassengerDTOAPI passengerDto)
        {
            var passengerDB = _mapper.Map<PassengerDB>(passengerDto);
            await _passengerRepository.AddPassengerAsync(passengerDB);
        }

        public async Task<PassengerDTOAPI> GetPassengerByIdAsync(int id)
        {
            var passenger = await _passengerRepository.GetPassengerByIdAsync(id.ToString());
            return _mapper.Map<PassengerDTOAPI>(passenger);
        }

        public async Task UpdatePassengerAsync(PassengerDTOAPI passengerDto)
        {
            var passengerDB = _mapper.Map<PassengerDB>(passengerDto);
            await _passengerRepository.UpdatePassengerAsync(passengerDB.Id, passengerDB);
        }

        public async Task DeletePassengerAsync(int id)
        {
            await _passengerRepository.DeletePassenger(id.ToString());
        }

        public async Task<IEnumerable<PassengerDTOAPI>> GetAllPassengersAsync()
        {
            var passengers = await _passengerRepository.GetAllPassengersAsync();
            return _mapper.Map<IEnumerable<PassengerDTOAPI>>(passengers);
        }
    }
}
