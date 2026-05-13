namespace api.Models;

public class ProjectTask
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public DateTime StartDate { get; set; }
  public DateTime Deadline { get; set; }
  public string Progress { get; set; } = "Todo";
  public string Priority { get; set; } = "Medium";
  public int ProjectId { get; set; }
  public Project? Project { get; set; }
}
