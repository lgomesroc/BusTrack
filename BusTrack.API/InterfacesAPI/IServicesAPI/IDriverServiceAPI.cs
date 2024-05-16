using BusTrack.BusTrack.API.DTOAPI;
using BusTrack.BusTrack.DB.Classes;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface IDriverServiceAPI
    {
        Task<IEnumerable<DriverDTOAPI>> GetAllDrivers();

        Task<DriverDTOAPI> GetDriverById(string id);

        Task<DriverDTOAPI> CreateDriver(DriverDTOAPI driver);

        Task<DriverDTOAPI> UpdateDriver(string id, DriverDTOAPI driver);

        Task<bool> DeleteDriver(string id);

        List<DriverDB> GetDrivers();
    }
}
