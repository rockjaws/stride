using api.Models;

namespace api.Repositories;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllProjectsAsync();
    Task<Project?> GetProjectByIdAsync(int id);
    Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId);
    Task AddProjectAsync(Project project);
    Task UpdateProjectAsync(Project project);
    Task DeleteProjectAsync(Project project);
    Task SaveChangesAsync();
}
