using api.Data;
using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    // Author: Nicolai and Oliver
    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    // Author: Nicolai and Oliver
    public async Task<IEnumerable<ProjectTask>> GetAllTasksAsync()
    {
        return await _context
            .ProjectTasks
            // Users are needed by the API response and by task update notifications.
            .Include(t => t.Users)
            .ToListAsync();
    }

    // Author: Nicolai and Oliver
    public async Task<IEnumerable<ProjectTask>> GetTasksByUserIdAsync(int userId)
    {
        return await _context
            .ProjectTasks.Include(t => t.Users)
            // This matches the client Tasks tab, which shows direct task assignments.
            .Where(t => t.Users.Any(u => u.Id == userId))
            .ToListAsync();
    }

    // Author: Nicolai and Oliver
    public async Task<ProjectTask?> GetTaskByIdAsync(int id)
    {
        return await _context
            .ProjectTasks.Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    // Author: Nicolai and Oliver
    public async Task AddTaskAsync(ProjectTask task)
    {
        await _context.ProjectTasks.AddAsync(task);
    }

    // Author: Nicolai and Oliver
    public async Task UpdateTaskAsync(ProjectTask task)
    {
        _context.ProjectTasks.Update(task);
        await Task.CompletedTask;
    }

    // Author: Nicolai and Oliver
    public async Task DeleteTaskAsync(ProjectTask task)
    {
        _context.ProjectTasks.Remove(task);
        await Task.CompletedTask;
    }

    // Author: Nicolai and Oliver
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
