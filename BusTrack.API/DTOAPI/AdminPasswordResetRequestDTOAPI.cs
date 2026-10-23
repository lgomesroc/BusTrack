namespace BusTrack.BusTrack.API.DTOAPI
{
    public sealed class AdminPasswordResetRequestDTOAPI
    {
        public string? AdminCurrentPassword { get; set; }
        public string? TargetEmail { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
