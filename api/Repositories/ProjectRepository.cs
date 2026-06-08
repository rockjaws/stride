using api.Data;
using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    // Author: Nicolai and Oliver
    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    // Author: Nicolai and Oliver
    public async Task<IEnumerable<Project>> GetAllProjectsAsync()
    {
        return await _context
            .Projects
            .AsSplitQuery()
            .Include(p => p.Tasks)
            .ThenInclude(p => p.Users)
            .Include(p => p.Users)
            .Include(p => p.ChatChannels)
            .ToListAsync();
    }

    // Author: Nicolai and Oliver
    public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId)
    {
        return await _context
            .Projects.AsSplitQuery()
            .Include(p => p.Tasks)
            .ThenInclude(p => p.Users)
            .Include(p => p.Users)
            .Include(p => p.ChatChannels)
            .Where(p => p.Users.Any(u => u.Id == userId))
            .ToListAsync();
    }

    // Author: Nicolai and Oliver
    public async Task<Project?> GetProjectByIdAsync(int? id)
    {
        return await _context
            .Projects.AsSplitQuery()
            .Include(p => p.Tasks)
                .ThenInclude(t => t.Users)
            .Include(p => p.Users)
            .Include(p => p.ChatChannels)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    // Author: Nicolai and Oliver
    public async Task AddProjectAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
    }

    // Author: Nicolai and Oliver
    public async Task UpdateProjectAsync(Project project)
    {
        _context.Projects.Update(project);
        await Task.CompletedTask;
    }

    // Author: Nicolai and Oliver
    public async Task DeleteProjectAsync(Project project)
    {
        _context.Projects.Remove(project);
        await Task.CompletedTask;
    }

    // Author: Nicolai and Oliver
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
