using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.Program.DatabaseServicesExtensionsProgram;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.json");

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontendPolicy", policy =>
            {
                policy
                    .WithOrigins("https://127.0.0.1:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddDatabaseServices(builder.Configuration);

        var connectionString =
            builder.Configuration.GetConnectionString("BusTrackDBConnection");

        var databaseName =
            builder.Configuration.GetConnectionString("DatabaseName")
            ?? builder.Configuration["ConnectionStrings:DatabaseName"];

        builder.Services.AddScoped<IAccountServiceAPI>(_ =>
            new AccountServiceAPI(connectionString!, databaseName!));

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
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
