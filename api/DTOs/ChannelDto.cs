namespace api.DTOs;

public class ChannelDto
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public int ProjectId { get; set; }
}
