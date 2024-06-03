namespace BusTrack.BusTrack.API.DTOAPI
{
    public class UpdatePasswordDTOAPI
    {
        public int UserId { get; set; }
        public string? NewPassword { get; set; }
        public DateTime UpdateDate { get; set; }

    }
}