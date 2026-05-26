using Microsoft.EntityFrameworkCore;
using api.Models;
using api.Models.Enums;

namespace api.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }
  public DbSet<Project> Projects { get; set; }
  public DbSet<ProjectTask> ProjectTasks { get; set; }
  public DbSet<User> Users { get; set; }
  public DbSet<ChatChannel> ChatChannels { get; set; }
  public DbSet<Message> Messages { get; set; }
  public DbSet<Notification> Notifications { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Store enum names so the database remains readable and matches the API JSON contract.
    modelBuilder.Entity<ProjectTask>()
      .Property(t => t.Progress)
      .HasConversion<string>();

    modelBuilder.Entity<ProjectTask>()
      .Property(t => t.Priority)
      .HasConversion<string>();
  }
}
