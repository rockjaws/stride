using api.Data;
using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

<<<<<<< HEAD
  public async Task<IEnumerable<Project>> GetAllProjectsAsync()
  {
    return await _context.Projects
      // Multiple collection includes can multiply rows; split queries keep the result shape predictable.
      .AsSplitQuery()
      .Include(p => p.Tasks)
      .Include(p => p.Users)
      .Include(p => p.ChatChannels)
      .ToListAsync();
  }

  public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId)
  {
    return await _context.Projects
      .AsSplitQuery()
      .Include(p => p.Tasks)
      .Include(p => p.Users)
      .Include(p => p.ChatChannels)
      // The client Projects tab should only see projects where the active user is a member.
      .Where(p => p.Users.Any(u => u.Id == userId))
      .ToListAsync();
  }

  public async Task<Project?> GetProjectByIdAsync(int id)
  {
    return await _context.Projects
      .AsSplitQuery()
      .Include(p => p.Tasks)
      .Include(p => p.Users)
      .Include(p => p.ChatChannels)
      .FirstOrDefaultAsync(p => p.Id == id);
  }
=======
    public async Task<IEnumerable<Project>> GetAllProjectsAsync()
    {
        return await _context.Projects
          .AsSplitQuery()
          .Include(p => p.Tasks)
          .Include(p => p.Users)
          .Include(p => p.ChatChannels)
          .ToListAsync();
    }

    public async Task<Project?> GetProjectByIdAsync(int id)
    {
        return await _context.Projects
          .AsSplitQuery()
          .Include(p => p.Tasks)
          .Include(p => p.Users)
          .Include(p => p.ChatChannels)
          .FirstOrDefaultAsync(p => p.Id == id);
    }
>>>>>>> 51c8af05bb2ee9efc82699f35cc1e81bca8b3b9c

    public async Task AddProjectAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
    }

    public async Task UpdateProjectAsync(Project project)
    {
        _context.Projects.Update(project);
        await Task.CompletedTask;
    }

    public async Task DeleteProjectAsync(Project project)
    {
        _context.Projects.Remove(project);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
