using api.Models.Enums;

namespace api.Models;

public class ProjectTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime Deadline { get; set; }
    public TaskProgress Progress { get; set; } = TaskProgress.Backlog;
    public TaskPriority Priority { get; set; } = TaskPriority.Normal;

    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    // Author: Nicolai and Oliver
    public List<User> Users { get; set; } = new();
}
