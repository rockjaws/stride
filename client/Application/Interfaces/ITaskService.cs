// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

using client.Domain.Enum;
using client.Domain.Models;

namespace client.Application.Interfaces;

public interface ITaskService
{
    // Author: Nicolai and Oliver
    Task<List<ProjectTask>> GetTasksAsync();
    // Author: Oliver
    Task<List<ProjectTask>> GetTasksAsync(int userId);
    // Author: Nicolai and Oliver
    Task<ProjectTask> CreateTaskAsync(ProjectTask task);
    // Author: Nicolai and Oliver
    Task UpdateTaskAsync(ProjectTask task);
    // Author: Nicolai and Oliver
    Task<ProjectTask> MoveTaskAsync(ProjectTask task, TaskProgress progress);
    // Author: Nicolai and Oliver
    Task DeleteTaskAsync(int id);
    // Author: Nicolai and Oliver
    void SortTasks();
    event EventHandler TasksChanged;
}
