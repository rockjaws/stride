// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

using client.Domain.Models;

namespace client.Application.Interfaces;

public interface IProjectService
{
    // Author: Nicolai and Oliver
    Task<List<Project>> GetProjectsAsync();
    // Author: Oliver
    Task<List<Project>> GetProjectsAsync(int userId);
    // Author: Nicolai and Oliver
    Task<Project> CreateProjectAsync(Project project, int userId);
    // Author: Oliver
    Task<Project> UpdateProjectAsync(Project project);
    // Author: Nicolai
    Task<ChatChannel> CreateChannelAsync(int projectId, string name);
    // Author: Nicolai and Oliver
    Task DeleteProjectAsync(int id);
    // Author: Oliver
    Task DeleteChannelAsync(int id, int projectId);
    // Author: Nicolai and Oliver
    Task SetProjectArchivedAsync(int id, bool isArchived);
    event EventHandler ProjectsChanged;
}
