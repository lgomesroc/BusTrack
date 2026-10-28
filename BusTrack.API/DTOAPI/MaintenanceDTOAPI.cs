using System.ComponentModel.DataAnnotations;

namespace BusTrack.BusTrack.API.DTOAPI
{
    public class MaintenanceDTOAPI
    {
        public string? Id { get; set; }
        [Required] public string BusId { get; set; } = string.Empty;
        public string BusNumber { get; set; } = string.Empty;
        [Required, StringLength(80)] public string ServiceType { get; set; } = string.Empty;
        [Required, StringLength(1000)] public string Description { get; set; } = string.Empty;
        public DateTime ServiceDate { get; set; }
        public DateTime ServiceStartTime { get; set; }
        [Range(1, 1440)] public int PlannedDurationMinutes { get; set; } = 60;
        public DateTime? CompletedDate { get; set; }
        [Range(typeof(decimal), "0", "999999999")] public decimal Cost { get; set; }
        [Required] public string Status { get; set; } = "Agendada";
    }
}
