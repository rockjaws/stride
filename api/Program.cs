using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "db", "stride.db"))}"));

// This tells the API: "Whenever a Controller asks for an IRepository, give them a Repository."
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChannelRepository, ChannelRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

builder.Services.AddControllers()
  .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
      });

var app = builder.Build();


app.MapControllers();

app.Run();
