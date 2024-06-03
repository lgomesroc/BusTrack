namespace BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI
{
    public interface IUpdatePasswordServiceAPI
    {

        void UpdatePassword(int userId, string newPassword);

    }
}