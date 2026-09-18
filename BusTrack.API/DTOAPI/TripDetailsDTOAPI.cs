namespace BusTrack.BusTrack.API.DTOAPI
{
    public class TripDetailsDTOAPI
    {
        public string? Id { get; set; }

        public BusDetailsDTOAPI? Bus { get; set; }

        public DriverDetailsDTOAPI? Driver { get; set; }

        public RouteDetailsDTOAPI? Route { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        public int Duration { get; set; }

        public int LimitPassengers { get; set; }

        public List<PassengerDetailsDTOAPI> Passengers { get; set; } = new();
    }

    public class BusDetailsDTOAPI
    {
        public string? Id { get; set; }

        public string? Number { get; set; }

        public string? LicensePlate { get; set; }

        public string? Model { get; set; }

        public int Capacity { get; set; }
    }

    public class DriverDetailsDTOAPI
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? LicenseNumber { get; set; }
    }

    public class RouteDetailsDTOAPI
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? Origin { get; set; }

        public string? Destination { get; set; }
    }

    public class PassengerDetailsDTOAPI
    {
        public string? Id { get; set; }

        public string? Name { get; set; }

        public string? Cpf { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }
    }
}
