using client.Domain.Models;

namespace client.Application.Interfaces;

public interface IProjectService
{
    Task<List<Project>> GetProjectsAsync();
    Task<List<Project>> GetProjectsAsync(int userId);
    Task<Project> CreateProjectAsync(Project project, int userId);
    Task DeleteProjectAsync(int id);
    Task SetProjectArchivedAsync(int id, bool isArchived);
}
