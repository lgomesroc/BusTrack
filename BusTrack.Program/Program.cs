using BusTrack.BusTrack.Program.DatabaseServicesExtensionsProgram;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços necessários ao contêiner.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDatabaseServices(builder.Configuration); // Adiciona os serviços do banco de dados

var app = builder.Build();

// Configuração do pipeline de requisição HTTP.
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
app.Use(async (context, next) =>
{
    await context.Response.WriteAsync("Custom middleware\n");
    await next();
});

app.Run();
