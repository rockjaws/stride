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

  public async Task<IEnumerable<Project>> GetAllProjectsAsync()
  {
    return await _context.Projects
      .Include(p => p.Tasks)
      .ToListAsync();
  }

  public async Task<Project?> GetProjectByIdAsync(int id)
  {
    return await _context.Projects.FindAsync(id);
  }

  public async Task AddProjectAsync(Project project)
  {
    await _context.Projects.AddAsync(project);
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
