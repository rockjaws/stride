using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Data;

public class AppDbContext : DbContext
{
  // The constructor accepts configuration options (like our SQLite connection string)
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }
  public DbSet<Project> Projects { get; set; }
  public DbSet<ProjectTask> ProjectTasks { get; set; }
  public DbSet<User> Users { get; set; }
  public DbSet<ChatChannel> ChatChannels { get; set; }
  public DbSet<Message> Messages { get; set; }
  public DbSet<Notification> Notifications { get; set; }
}
