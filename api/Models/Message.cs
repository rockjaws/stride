namespace api.Models;

public class Message
{
  public int Id { get; set; }
  public string Text { get; set; } = string.Empty;
  public DateTime Time { get; set; } = DateTime.Now;
  public int ChannelId { get; set; }
  public ChatChannel? Channel { get; set; }
  public int UserId { get; set; }
  public User? User { get; set; }
}
