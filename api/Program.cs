using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "db", "stride.db"))}"));

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChannelRepository, ChannelRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

var app = builder.Build();

// Make local startup self-contained: create the shared SQLite folder and apply pending migrations.
using (var scope = app.Services.CreateScope())
{
<<<<<<< HEAD
  var services = scope.ServiceProvider;
  try
  {
    var context = services.GetRequiredService<AppDbContext>();

    var dbDirectory = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "db"));
    if (!Directory.Exists(dbDirectory))
=======
    var services = scope.ServiceProvider;
    try
>>>>>>> 51c8af05bb2ee9efc82699f35cc1e81bca8b3b9c
    {
        var context = services.GetRequiredService<AppDbContext>();

<<<<<<< HEAD
    context.Database.Migrate();
  }
  catch (Exception ex)
  {
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred creating the DB.");
  }
=======
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
>>>>>>> 51c8af05bb2ee9efc82699f35cc1e81bca8b3b9c
}

app.MapControllers();

app.Run();
