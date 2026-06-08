// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

using client.Application.Interfaces;
using client.Domain.Enum;

namespace client.Domain.Models;

public class ProjectTask : ITask
{
    public int? Id { get; }
    public string Title { get; }
    public string Description { get; }
    public DateTime StartDate { get; }
    public DateTime Deadline { get; }
    public TaskProgress Progress { get; }
    public TaskPriority Priority { get; }
    public int? ProjectId { get; }
    public List<User>? UsersAssigned { get; set; } = [];

    // Author: Oliver
    public ProjectTask(
        int? id,
        string title,
        string description,
        DateTime startDate,
        DateTime deadline,
        TaskProgress progress,
        TaskPriority priority,
        int? projectId = null
    )
    {
        Id = id;
        Title = title;
        Description = description;
        StartDate = startDate;
        Deadline = deadline;
        Progress = progress;
        Priority = priority;
        ProjectId = projectId;
    }
}
