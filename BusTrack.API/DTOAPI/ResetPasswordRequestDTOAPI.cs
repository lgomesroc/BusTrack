namespace BusTrack.BusTrack.API.DTOAPI;

public sealed class ResetPasswordRequestDTOAPI
{
    public string? Email { get; set; }

    public string? RecoveryCode { get; set; }

    public string? NewPassword { get; set; }

    public string? ConfirmPassword { get; set; }
}
