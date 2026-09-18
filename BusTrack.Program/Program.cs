using System.Reflection;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.BusTrack.DB.RepositoriesDB;
using BusTrack.BusTrack.Program.DatabaseServicesExtensionsProgram;
using MongoDB.Driver;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile(
            "appsettings.json");

        builder.Services
            .AddControllers()
            .AddApplicationPart(
                typeof(
                    BusTrack.BusTrack.API.ControllersAPI.BusControllerAPI)
                    .Assembly);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();

        builder.Services.AddAutoMapper(
            (serviceProvider, mapperConfiguration) =>
            {
                mapperConfiguration.AddProfile<
                    BusTrack.BusTrack.API.MappingsAPI.MappingProfileAPI>();
            },
            Array.Empty<Assembly>());

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                "FrontendPolicy",
                policy =>
                {
                    policy
                        .WithOrigins(
                            "https://127.0.0.1:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        builder.Services.AddDatabaseServices(
            builder.Configuration);

        var connectionString =
            builder.Configuration.GetConnectionString(
                "BusTrackDBConnection");

        var databaseName =
            builder.Configuration.GetConnectionString(
                "DatabaseName")
            ?? builder.Configuration[
                "ConnectionStrings:DatabaseName"];

        builder.Services.AddSingleton<IMongoDatabase>(
            serviceProvider =>
            {
                var client =
                    serviceProvider
                        .GetRequiredService<IMongoClient>();

                return client.GetDatabase(
                    databaseName);
            });

        builder.Services.AddScoped<
            IUserRepositoryDB,
            UserRepositoryDB>();

        builder.Services.AddScoped<
            IUserAuthenticationServiceAPI,
            UserAuthenticationServiceAPI>();

        builder.Services.AddScoped<
            IAccountServiceAPI>(
            serviceProvider =>
                new AccountServiceAPI(
                    connectionString!,
                    databaseName!));

        builder.Services.AddScoped<
            IBusRepositoryDB,
            BusRepositoryDB>();

        builder.Services.AddScoped<
            IBusServiceAPI,
            BusServiceAPI>();

        builder.Services.AddScoped<
            IDriverRepositoryDB,
            DriverRepositoryDB>();

        builder.Services.AddScoped<
            IDriverServiceAPI,
            DriverServiceAPI>();

        builder.Services.AddScoped<
            IRouteRepositoryDB,
            RouteRepositoryDB>();

        builder.Services.AddScoped<
            IRouteServiceAPI,
            RouteServiceAPI>();

        builder.Services.AddScoped<
            ITripRepositoryDB,
            TripRepositoryDB>();

        builder.Services.AddScoped<
            ITripServiceAPI,
            TripServiceAPI>();

        builder.Services.AddScoped<
            ITripPassengerRepositoryDB,
            TripPassengerRepositoryDB>();

        builder.Services.AddScoped<
            ITripsPassengerServiceAPI,
            TripsPassengerServiceAPI>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();

            app.UseSwaggerUI();

            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");

            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseCors("FrontendPolicy");

        app.UseAuthorization();

        app.MapControllers();

        app.MapControllerRoute(
            name: "default",
            pattern:
                "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
