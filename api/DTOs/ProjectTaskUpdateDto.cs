namespace api.DTOs;

public class ProjectTaskUpdateDto
{
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime Deadline { get; set; }
  public string Progress { get; set; } = "Todo";
  public string Priority { get; set; } = "Medium";
}
