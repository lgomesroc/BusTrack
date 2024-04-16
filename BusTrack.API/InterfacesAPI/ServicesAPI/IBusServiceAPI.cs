using BusTrack.BusTrack.API.DTOAPI;

namespace BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI
{
    public interface IBusServiceAPI
    {
        Task<IEnumerable<BusDTOAPI>> GetAllBuses();

        Task<BusDTOAPI> GetBusById(string id);

        Task<BusDTOAPI> CreateBus(BusDTOAPI bus);

        Task<BusDTOAPI> UpdateBus(string id, BusDTOAPI bus);

        Task<bool> DeleteBus(string id);
    }
}
