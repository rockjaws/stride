using client.Domain.Models;

namespace client.Application.Interfaces;

public interface IProjectService
{
    Task<List<Project>> GetProjectsAsync();
    Task<List<Project>> GetProjectsAsync(int userId);
    Task<Project> CreateProjectAsync(Project project);
    Task DeleteProjectAsync(int id);
    Task ArchiveProjectAsync(int id);
    Task UnArchiveProjectAsync(int id);
}
