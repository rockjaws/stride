using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ensures Enums are sent/received as strings (e.g., "Backlog") instead of integers
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configure the SQLite database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "db", "stride.db"))}"));

// This tells the API: "Whenever a Controller asks for an IRepository, give them a Repository."
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChannelRepository, ChannelRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

var app = builder.Build();

// Automatically apply migrations and create the database folder/file on startup
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
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

app.MapControllers();

app.Run();
