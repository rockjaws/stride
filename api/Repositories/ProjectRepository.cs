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
