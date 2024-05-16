using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.Program.MiddlewaresProgram;


namespace BusTrack.BusTrack.Program.ExtensionsProgram
{
    public static class ServiceExtensions
    {
        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<IPassengerServiceAPI, PassengerServiceAPI>();
        }

        public static void AddCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlingMiddlewareProgram>();
        }
    }
}
