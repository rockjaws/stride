namespace api.Models;

public class Notification
{
  public int Id { get; set; }
  public string Text { get; set; } = string.Empty;
  public bool IsRead { get; set; }
  public DateTime Time { get; set; } = DateTime.Now;

  public int UserId { get; set; }
  public User? User { get; set; }

  public int? TaskId { get; set; }
  public ProjectTask? ProjectTask { get; set; }
}
