using BusTrack.BusTrack.API.InterfacesAPI.ServicesAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.BusTrack.DB.RepositoriesDB;

namespace BusTrack.BusTrack.Program.ExtensionsProgram
{
    public static class ServiceExtensionProgram
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IPassengerServiceAPI, PassengerServiceAPI>();
            services.AddScoped<IPassengerRepositoryDB, PassengerRepositoryDB>();
        }
    }
}
