namespace BusTrack.BusTrack.API.DTOAPI
{
    public class OperationalReportDTOAPI
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalTrips { get; set; }
        public int CompletedTrips { get; set; }
        public int CancelledTrips { get; set; }
        public int ScheduledTrips { get; set; }
        public int InProgressTrips { get; set; }
        public decimal TotalMaintenanceCost { get; set; }
        public List<BusUtilizationReportDTOAPI> BusUtilization { get; set; } = new();
        public List<DriverTripReportDTOAPI> DriverTrips { get; set; } = new();
        public List<MaintenanceCostReportDTOAPI> MaintenanceCosts { get; set; } = new();
    }

    public class BusUtilizationReportDTOAPI
    {
        public string BusId { get; set; } = string.Empty;
        public string BusNumber { get; set; } = string.Empty;
        public int TripCount { get; set; }
        public int CompletedTripCount { get; set; }
    }

    public class DriverTripReportDTOAPI
    {
        public string DriverId { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public int TripCount { get; set; }
        public int CompletedTripCount { get; set; }
    }

    public class MaintenanceCostReportDTOAPI
    {
        public string BusId { get; set; } = string.Empty;
        public string BusNumber { get; set; } = string.Empty;
        public decimal TotalCost { get; set; }
        public int MaintenanceCount { get; set; }
    }
}