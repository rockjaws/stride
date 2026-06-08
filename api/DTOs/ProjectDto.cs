namespace api.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime Deadline { get; set; }
    public bool IsArchived { get; set; } = false;

    // Author: Nicolai and Oliver
    public List<ChannelDto> ChatChannels { get; set; } = new();
    // Author: Nicolai and Oliver
    public List<ProjectTaskDto> Tasks { get; set; } = new();
    // Author: Nicolai and Oliver
    public List<UserDto> Users { get; set; } = new();
}
