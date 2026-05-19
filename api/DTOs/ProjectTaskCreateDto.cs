using api.Models.Enums;

namespace api.DTOs;

public class ProjectTaskCreateDto
{
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime Deadline { get; set; }
  public TaskPriority Priority { get; set; } = TaskPriority.Normal;

  public int ProjectId { get; set; }
}
