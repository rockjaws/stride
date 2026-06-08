using api.Models.Enums;

namespace api.DTOs;

public class ProjectTaskDto
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime Deadline { get; set; }
  public TaskProgress Progress { get; set; } = TaskProgress.Backlog;
  public TaskPriority Priority { get; set; } = TaskPriority.Normal;

  public int? ProjectId { get; set; }
  public List<UserDto> Users { get; set; } = new();
}
