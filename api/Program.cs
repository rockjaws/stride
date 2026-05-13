using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=stride.db"));

// This tells the API: "Whenever a Controller asks for an IRepository, give them a Repository."
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();


app.MapControllers();

app.Run();
