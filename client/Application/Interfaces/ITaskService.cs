using client.Domain.Enum;
using client.Domain.Models;

namespace client.Application.Interfaces;

public interface ITaskService
{
    Task<List<ProjectTask>> GetTasksAsync();
    Task<ProjectTask> CreateTaskAsync(ProjectTask task);
    Task UpdateTaskAsync(ProjectTask task);
    Task<ProjectTask> MoveTaskAsync(ProjectTask task, TaskProgress progress);
    Task DeleteTaskAsync(int id);
    void SortTasks();
}
