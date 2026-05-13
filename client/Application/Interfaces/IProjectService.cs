using client.Domain.Models;

namespace client.Application.Interfaces;

public interface IProjectService
{
    Task<List<Project>> GetProjectsAsync();
    Task CreateProjectAsync(Project project);
    Task DeleteProjectAsync(int id);
}
