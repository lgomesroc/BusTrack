using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BusTrack.BusTrack.Program.DatabaseServicesExtensionsProgram
{
    public static class DatabaseServicesExtensionsProgram
    {
            public static void AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMongoClient>(sp =>
            {
                var connectionString = configuration.GetConnectionString("BusTrackDBConnection");
                var connectionDB = new BusTrack.DB.ConnectionsDB.ConnectionDB();
                var database = connectionDB.Connect(connectionString, "nome-do-seu-banco-de-dados-aqui");
                return database.Client;
            });
        }
    }
} 
