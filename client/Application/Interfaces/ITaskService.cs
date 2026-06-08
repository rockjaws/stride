// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using client.Domain.Enum;
using client.Domain.Models;

namespace client.Application.Interfaces;

public interface ITaskService
{
    // Author: Nicolaj and Oliver
    Task<List<ProjectTask>> GetTasksAsync();
    // Author: Oliver
    Task<List<ProjectTask>> GetTasksAsync(int userId);
    // Author: Nicolaj and Oliver
    Task<ProjectTask> CreateTaskAsync(ProjectTask task);
    // Author: Nicolaj and Oliver
    Task UpdateTaskAsync(ProjectTask task);
    // Author: Nicolaj and Oliver
    Task<ProjectTask> MoveTaskAsync(ProjectTask task, TaskProgress progress);
    // Author: Nicolaj and Oliver
    Task DeleteTaskAsync(int id);
    // Author: Nicolaj and Oliver
    void SortTasks();
    event EventHandler TasksChanged;
}
