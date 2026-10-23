namespace BusTrack.BusTrack.API.DTOAPI;

public sealed class ForgotPasswordRequestDTOAPI
{
    public string? Email { get; set; }

    public string? NewPassword { get; set; }

    public string? ConfirmPassword { get; set; }
}
