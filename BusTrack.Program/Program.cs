
using System.Reflection;
using System.Text;
using BusTrack.BusTrack.API.InterfacesAPI.IServicesAPI;
using BusTrack.BusTrack.API.ServicesAPI;
using BusTrack.BusTrack.DB.InterfacesDB.IRepositoriesDB;
using BusTrack.BusTrack.DB.RepositoriesDB;
using BusTrack.BusTrack.Program.DatabaseServicesExtensionsProgram;
using BusTrack.BusTrack.Program.ExtensionsProgram;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MongoDB.Driver;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers().AddApplicationPart(typeof(BusTrack.BusTrack.API.ControllersAPI.BusControllerAPI).Assembly);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Name = "Authorization", Description = "Informe o token JWT no formato: Bearer {seu_token}", In = ParameterLocation.Header, Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT" });
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] });
        });
        builder.Services.AddAutoMapper((serviceProvider, mapperConfiguration) => mapperConfiguration.AddProfile<BusTrack.BusTrack.API.MappingsAPI.MappingProfileAPI>(), Array.Empty<Assembly>());
        builder.Services.AddCors(options => options.AddPolicy("FrontendPolicy", policy => policy.WithOrigins("http://localhost:4200", "https://localhost:4200", "http://127.0.0.1:4200", "https://127.0.0.1:4200", "https://bustrack-frontend-2cbs.onrender.com").AllowAnyHeader().AllowAnyMethod()));

        var jwtKey = builder.Configuration["JwtSettings:SecretKey"];
        var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
        var jwtAudience = builder.Configuration["JwtSettings:Audience"];
        if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32) throw new InvalidOperationException("Configure JwtSettings__SecretKey com pelo menos 32 bytes. Não use uma chave pública ou de exemplo em produção.");
        if (string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience)) throw new InvalidOperationException("Configure JwtSettings__Issuer e JwtSettings__Audience.");
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidIssuer = jwtIssuer, ValidateAudience = true, ValidAudience = jwtAudience, ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), ValidateLifetime = true, ClockSkew = TimeSpan.FromSeconds(30) });
        builder.Services.AddAuthorization(options => options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        builder.Services.AddDatabaseServices(builder.Configuration);
        BusTrack.BusTrack.Program.ExtensionsProgram.ServiceExtensionProgram.AddCustomServices(builder.Services);

        var connectionString = builder.Configuration.GetConnectionString("BusTrackDBConnection");
        var databaseName = builder.Configuration.GetConnectionString("DatabaseName") ?? builder.Configuration["ConnectionStrings:DatabaseName"];
        builder.Services.AddSingleton<IMongoDatabase>(serviceProvider => serviceProvider.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<IUserRepositoryDB, UserRepositoryDB>();
        builder.Services.AddScoped<IUserAuthenticationServiceAPI, UserAuthenticationServiceAPI>();
        builder.Services.AddScoped<IJwtTokenServiceAPI, JwtTokenServiceAPI>();
        builder.Services.AddScoped<IAccountServiceAPI>(serviceProvider => new AccountServiceAPI(connectionString!, databaseName!));
        builder.Services.AddScoped<IBusRepositoryDB, BusRepositoryDB>();
        builder.Services.AddScoped<IBusServiceAPI, BusServiceAPI>();
        builder.Services.AddScoped<IDriverRepositoryDB, DriverRepositoryDB>();
        builder.Services.AddScoped<IDriverServiceAPI, DriverServiceAPI>();
        builder.Services.AddScoped<IRouteRepositoryDB, RouteRepositoryDB>();
        builder.Services.AddScoped<IRouteServiceAPI, RouteServiceAPI>();
        builder.Services.AddScoped<ITripRepositoryDB, TripRepositoryDB>();
        builder.Services.AddScoped<ITripAuditRepositoryDB, TripAuditRepositoryDB>();
        builder.Services.AddScoped<ITripServiceAPI, TripServiceAPI>();
        builder.Services.AddScoped<ITripPassengerRepositoryDB, TripPassengerRepositoryDB>();
        builder.Services.AddScoped<ITripsPassengerServiceAPI, TripsPassengerServiceAPI>();
        builder.Services.AddScoped<IMaintenanceRepositoryDB, MaintenanceRepositoryDB>();
        builder.Services.AddScoped<IMaintenanceAuditRepositoryDB, MaintenanceAuditRepositoryDB>();
        builder.Services.AddScoped<IMaintenanceServiceAPI, MaintenanceServiceAPI>();

        var app = builder.Build();
        app.UseAuthentication();
        if (app.Environment.IsDevelopment())
        {
            app.UseWhen(context => context.Request.Path.StartsWithSegments("/swagger/v1/swagger.json"), swaggerApp => swaggerApp.Use(async (context, next) => { if (context.User.Identity?.IsAuthenticated != true) { context.Response.StatusCode = StatusCodes.Status401Unauthorized; return; } if (!context.User.IsInRole("ADMIN")) { context.Response.StatusCode = StatusCodes.Status403Forbidden; return; } await next(); }));
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        app.UseCors("FrontendPolicy");
        app.UseAuthorization();
        app.MapControllers();
        app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
        app.Run();
    }
}
