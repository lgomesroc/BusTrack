
namespace BusTrack.BusTrack.API.DTOAPI
{
    public sealed class ChangePasswordRequestDTOAPI
    {
        public string? Email { get; set; }

        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }

        public string? ConfirmPassword { get; set; }
    }
}
