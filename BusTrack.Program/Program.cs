var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure the API documentation (Swagger/OpenAPI).
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build the application.
var app = builder.Build();

// Configure the HTTP request pipeline.
ConfigurePipeline(app);

// Run the application.
app.Run();

// Configuration method for the HTTP request pipeline.
void ConfigurePipeline(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        // Enable Swagger and Swagger UI for development environment.
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Redirect HTTP requests to HTTPS.
    app.UseHttpsRedirection();

    // Enable authorization.
    app.UseAuthorization();

    // Map controllers to HTTP endpoints.
    app.MapControllers();
}
