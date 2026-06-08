using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;

using api.Data;
using api.Infrastructure.Logging;
using api.Repositories;

var builder = WebApplication.CreateBuilder(args);
var logFilePath = FileLoggerProvider.GetDefaultLogFilePath();
builder.Logging.AddProvider(new FileLoggerProvider(logFilePath));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ensures Enums are sent/received as strings (e.g., "Backlog") instead of integers
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "db", "stride.db"))}"));

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChannelRepository, ChannelRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

var app = builder.Build();
app.Logger.LogInformation("API starting. Log file: {LogFilePath}", logFilePath);

// Make local startup self-contained: create the shared SQLite folder and apply pending migrations.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // 1. Ensure the '../db' directory exists so SQLite doesn't crash
        var dbDirectory = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "db"));
        if (!Directory.Exists(dbDirectory))
        {
            Directory.CreateDirectory(dbDirectory);
        }

        // 2. Apply pending migrations (this will also create the stride.db file if missing)
        context.Database.Migrate();
        app.Logger.LogInformation("Database migration check completed.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating or migrating the DB.");
    }
}

app.Use(async (context, next) =>
        {
            if (!context.Request.Headers.TryGetValue("X-Stride-API-Key", out var requestApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key missing.");
                return;
            }

            var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = configuration["ApiSettings:ApiKey"];

            if (string.IsNullOrEmpty(apiKey) || !apiKey.Equals(requestApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            await next();
        });

app.MapControllers();

app.Run();
