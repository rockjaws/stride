namespace api.DTOs;

public class ProjectUpdateDto
{
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime Deadline { get; set; }
  public List<int>? UserIds { get; set; }
}
