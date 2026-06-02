using client.Domain.Models;

namespace client.Application.Interfaces;

public interface IProjectService
{
    Task<List<Project>> GetProjectsAsync();
    Task<List<Project>> GetProjectsAsync(int userId);
    Task<Project> CreateProjectAsync(Project project, int userId);
    Task<ChatChannel> CreateChannelAsync(int projectId, string name);
    Task DeleteProjectAsync(int id);
    Task DeleteChannelAsync(int id, int projectId);
    Task SetProjectArchivedAsync(int id, bool isArchived);
}
