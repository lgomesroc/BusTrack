using BusTrack.BusTrack.API.DTOModelAPI;

namespace BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI
{
    public interface IDriverServiceAPI
    {
        Task<IEnumerable<DriverDTOAPI>> GetAllDrivers();

        Task<DriverDTOAPI> GetDriverById(int id);

        Task<DriverDTOAPI> CreateDriver(DriverDTOAPI driver);

        Task<DriverDTOAPI> UpdateDriver(int id, DriverDTOAPI driver);

        Task<bool> DeleteDriver(int id);
    }
}
