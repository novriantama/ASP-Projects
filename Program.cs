using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using ASPProjects.Business.Services;
using ASPProjects.Data.Repositories;
using ASPProjects.Database;

// Load environment variables from .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure App URL (Host & Port) from environment variables
var appHost = Environment.GetEnvironmentVariable("APP_HOST") ?? "localhost";
var appPort = Environment.GetEnvironmentVariable("APP_PORT") ?? "5050";
builder.WebHost.UseUrls($"http://{appHost}:{appPort}");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Build SQL Server connection string from environment variables
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "1433";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "ProjectsDb";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
var dbPass = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "YourStrong@Password123";

var connectionString = $"Server={dbHost},{dbPort};Database={dbName};User Id={dbUser};Password={dbPass};TrustServerCertificate=True;";

// Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    }));

// JWT Authentication Configuration
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "ThisIsAStrongAndSecureSecretKeyForASPProjectsApplication2026!";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "ASPProjectsApi";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "ASPProjectsClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Dependency Injection - Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

// Dependency Injection - Services
builder.Services.AddSingleton<IIdProtector, IdProtector>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// HttpClient Registration for WeatherService
builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    client.BaseAddress = new Uri("https://api.weatherapi.com/v1/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

// Automatically apply database migrations on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var dbContext = services.GetRequiredService<AppDbContext>();

    var retries = 10;
    while (retries > 0)
    {
        try
        {
            dbContext.Database.Migrate();
            logger.LogInformation("Database '{DbName}' migrations applied successfully.", dbName);
            break;
        }
        catch (Exception ex)
        {
            retries--;
            if (retries == 0)
            {
                logger.LogError(ex, "Failed to apply migrations to database '{DbName}' after multiple attempts.", dbName);
                throw;
            }
            logger.LogWarning("Waiting for database '{DbName}' to be ready... ({Retries} retries left)", dbName, retries);
            Thread.Sleep(3000);
        }
    }
}

// Configure OpenAPI & Interactive API Documentation
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "ASP-Projects API Documentation";
        options.Theme = ScalarTheme.Purple;
        options.DefaultHttpClient = new(ScalarTarget.Http, ScalarClient.Http11);
    });

    // Optional redirect from /swagger to /scalar/v1 for convenience
    app.MapGet("/swagger", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
