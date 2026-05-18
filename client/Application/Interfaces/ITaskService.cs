using client.Domain.Models;

namespace client.Application.Interfaces;

public interface ITaskService
{
  Task<List<ProjectTask>> GetTasksAsync();
  Task<ProjectTask> CreateTaskAsync(ProjectTask task);
  Task UpdateTaskAsync(ProjectTask task);
  Task DeleteTaskAsync(int id);
  void SortTasks();
}
