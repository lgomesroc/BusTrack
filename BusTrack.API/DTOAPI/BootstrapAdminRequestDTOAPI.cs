namespace BusTrack.BusTrack.API.DTOAPI
{
    public sealed class BootstrapAdminRequestDTOAPI
    {
        public string? BootstrapSecret { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? CPF { get; set; }
        public string? Login { get; set; }
    }
}
