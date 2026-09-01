using Microsoft.EntityFrameworkCore;
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

// Dependency Injection - Repositories & Services
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
