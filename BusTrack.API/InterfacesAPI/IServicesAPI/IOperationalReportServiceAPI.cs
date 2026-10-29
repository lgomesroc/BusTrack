using BusTrack.BusTrack.API.DTOAPI;

namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface IOperationalReportServiceAPI
    {
        Task<OperationalReportDTOAPI> GetReportAsync(
            DateTime startDate,
            DateTime endDate);
    }
}