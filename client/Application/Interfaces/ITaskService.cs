using client.Domain.Enum;
using client.Domain.Models;

namespace client.Application.Interfaces;

public interface ITaskService
{
    ProjectTask CreateTask(
        string title,
        string description,
        DateTime startDate,
        DateTime deadline,
        TaskProgress status,
        TaskPriority priority
    );
    void AssignTask();
    void UpdateTask();
    void SortTasks();
}
