using MongoDB.Driver;

namespace BusTrack.BusTrack.Program.DatabaseServicesExtensionsProgram
{
    public static class DatabaseServicesExtensionsProgram
    {
        public static void AddDatabaseServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IMongoClient>(sp =>
            {
                var connectionString =
                    configuration.GetConnectionString("BusTrackDBConnection");

                return new MongoClient(connectionString);
            });
        }
    }
}
