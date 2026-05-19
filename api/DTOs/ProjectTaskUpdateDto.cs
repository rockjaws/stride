using api.Models.Enums;

namespace api.DTOs;

public class ProjectTaskUpdateDto
{
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime Deadline { get; set; }
  public TaskProgress Progress { get; set; } = TaskProgress.Backlog;
  public TaskPriority Priority { get; set; } = TaskPriority.Normal;
}
