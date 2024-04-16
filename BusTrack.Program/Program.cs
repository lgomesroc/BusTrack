using BusTrack.BusTrack.Program;
using BusTrack.BusTrack.Program.ExtensionsProgram;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços necessários ao contêiner.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adiciona os serviços e middleware personalizados.
builder.Services.AddCustomServices();

// Constrói a aplicação.
var app = builder.Build();

// Configuração do pipeline de requisição HTTP.
ConfigurePipeline(app);

// Executa a aplicação.
app.Run();

// Método de configuração do pipeline de requisição HTTP.
void ConfigurePipeline(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        // Habilita o Swagger e Swagger UI para ambiente de desenvolvimento.
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Redireciona requisições HTTP para HTTPS.
    app.UseHttpsRedirection();

    // Habilita autorização.
    app.UseAuthorization();

    // Mapeia os controllers para os endpoints HTTP.
    app.MapControllers();

    // Adiciona middleware personalizado.
    app.AddCustomMiddleware();
}   