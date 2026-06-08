// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using client.Domain.Models;

namespace client.Application.Interfaces;

public interface IProjectService
{
    // Author: Nicolaj and Oliver
    Task<List<Project>> GetProjectsAsync();
    // Author: Oliver
    Task<List<Project>> GetProjectsAsync(int userId);
    // Author: Nicolaj and Oliver
    Task<Project> CreateProjectAsync(Project project, int userId);
    // Author: Oliver
    Task<Project> UpdateProjectAsync(Project project);
    // Author: Nicolaj
    Task<ChatChannel> CreateChannelAsync(int projectId, string name);
    // Author: Nicolaj and Oliver
    Task DeleteProjectAsync(int id);
    // Author: Oliver
    Task DeleteChannelAsync(int id, int projectId);
    // Author: Nicolaj and Oliver
    Task SetProjectArchivedAsync(int id, bool isArchived);
    event EventHandler ProjectsChanged;
}
