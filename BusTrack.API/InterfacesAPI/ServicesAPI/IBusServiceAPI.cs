using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI
{
    public interface IBusServiceAPI
    {
        Task<IEnumerable<BusDTOAPI>> GetAllBuses();

        Task<BusDTOAPI> GetBusById(string id);

        Task<BusDTOAPI> CreateBus(BusDTOAPI bus);

        Task<BusDTOAPI> UpdateBus(string id, BusDTOAPI bus);

        Task<bool> DeleteBus(string id);

        List<BusDB> GetBuses(); 

    }
}
