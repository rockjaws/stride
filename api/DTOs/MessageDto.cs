namespace api.DTOs;

public class MessageDto
{
  public int Id { get; set; }
  public string Text { get; set; } = string.Empty;
  public DateTime Time { get; set; } = DateTime.Now;
  public int ChannelId { get; set; }

  public int SenderUserId { get; set; }
  public string SenderUsername { get; set; } = string.Empty;
}
