namespace api.DTOs;

public class NotificationDto
{
  public int Id { get; set; }
  public string Text { get; set; } = string.Empty;
  public bool IsRead { get; set; }
  public DateTime Time { get; set; } = DateTime.Now;

  public int? TaskId { get; set; }
}
