using api.Models;

namespace api.Repositories;

public interface ITaskRepository
{
  Task<IEnumerable<ProjectTask>> GetAllTasksAsync();
  Task<ProjectTask?> GetTaskByIdAsync(int id);
  Task AddTaskAsync(ProjectTask task);
  Task UpdateTaskAsync(ProjectTask task);
  Task DeleteTaskAsync(ProjectTask task);
  Task SaveChangesAsync();
}
