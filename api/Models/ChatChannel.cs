namespace api.Models;

public class ChatChannel
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public int ProjectId { get; set; }
  public Project? Project { get; set; }
  // Author: Nicolai and Oliver
  public List<Message> Messages { get; set; } = new();
}
