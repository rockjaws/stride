namespace api.DTOs;

public class MessageCreateDto
{
  public string Text { get; set; } = string.Empty;
  public int ChannelId { get; set; }
  public int UserId { get; set; }
}
