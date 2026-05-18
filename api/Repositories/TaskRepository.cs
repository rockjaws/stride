using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class TaskRepository : ITaskRepository
{
  private readonly AppDbContext _context;

  public TaskRepository(AppDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<ProjectTask>> GetAllTasksAsync()
  {
    return await _context.ProjectTasks
      .Include(t => t.Users)
      .ToListAsync();
  }

  public async Task<ProjectTask?> GetTaskByIdAsync(int id)
  {
    return await _context.ProjectTasks.FindAsync(id);
  }

  public async Task AddTaskAsync(ProjectTask task)
  {
    await _context.ProjectTasks.AddAsync(task);
  }

  public async Task UpdateTaskAsync(ProjectTask task)
  {
    _context.ProjectTasks.Update(task);
    await Task.CompletedTask;
  }

  public async Task DeleteTaskAsync(ProjectTask task)
  {
    _context.ProjectTasks.Remove(task);
    await Task.CompletedTask;
  }

  public async Task SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}
