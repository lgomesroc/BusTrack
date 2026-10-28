using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTrack.BusTrack.API.ControllersAPI
{
    [Route("api/OperationalReports")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class OperationalReportControllerAPI : ControllerBase
    {
        private readonly IOperationalReportServiceAPI _reportService;

        public OperationalReportControllerAPI(
            IOperationalReportServiceAPI reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var report = await _reportService.GetReportAsync(startDate, endDate);
                return Ok(report);
            }
            catch (ArgumentException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }
    }
}