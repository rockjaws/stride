using client.Application.Interfaces;
using client.Domain.Enum;
using client.Domain.Models;

namespace client.Application.Services;

public class TaskService : ITaskService
{
    public ProjectTask CreateTask(
        string title,
        string description,
        DateTime startDate,
        DateTime deadline,
        TaskProgress status,
        TaskPriority priority
    )
    {
        ProjectTask task = new ProjectTask(
            1,
            title,
            description,
            startDate,
            deadline,
            status,
            priority
        );
        return task;
    }

    public void UpdateTask() { }

    public void AssignTask() { }

    public void SortTasks() { }
}
