using BusTrack.BusTrack.Program.DatabaseServicesExtensionsProgram;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDatabaseServices(builder.Configuration);
        builder.Configuration.AddJsonFile("appsettings.json"); 

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


        var backendUrl = builder.Configuration["BackendUrl"];

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Use(async (context, next) =>
        {
            await context.Response.WriteAsync("Custom middleware\n");
            await next();
        });

        app.Run();
    }
}