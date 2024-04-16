using BusTrack.BusTrack.API.DTOModelAPI;

namespace BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI
{
    public interface IBusServiceAPI
    {
        Task<IEnumerable<BusDTOAPI>> GetAllBuses();

        Task<BusDTOAPI> GetBusById(int id);

        Task<BusDTOAPI> CreateBus(BusDTOAPI bus);

        Task<BusDTOAPI> UpdateBus(int id, BusDTOAPI bus);

        Task<bool> DeleteBus(int id);
    }
}
