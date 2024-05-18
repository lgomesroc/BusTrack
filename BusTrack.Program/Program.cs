using BenchmarkDotNet.Running;
using BusTrack.BusTrack.Program.DatabaseServicesExtensionsProgram;
using BusTrack.BusTrack.Tests.PerfomanceTests;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var summary = BenchmarkRunner.Run<PassengerServiceAPIPerformanceTests>();
        var summary2 = BenchmarkRunner.Run<RouteServiceAPIPerformanceTests>();
        var passengerSummary = BenchmarkRunner.Run<PassengerServiceAPIPerformanceTests>();

        // Adiciona os serviços necessários ao contêiner.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDatabaseServices(builder.Configuration); // Adiciona os serviços do banco de dados
        builder.Configuration.AddJsonFile("appsettings.json"); // Carrega as configurações do arquivo appsettings.json

        var app = builder.Build();

        // Configuração do pipeline de requisição HTTP.
        if (app.Environment.IsDevelopment())
        {
            // Habilita o Swagger e Swagger UI para ambiente de desenvolvimento.
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // Redireciona requisições HTTP para HTTPS.
        app.UseHttpsRedirection();


        var backendUrl = builder.Configuration["BackendUrl"];

        // Habilita autorização.
        app.UseAuthorization();

        // Mapeia os controllers para os endpoints HTTP.
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        // Adiciona middleware personalizado.
        app.Use(async (context, next) =>
        {
            await context.Response.WriteAsync("Custom middleware\n");
            await next();
        });

        app.Run();
    }
}