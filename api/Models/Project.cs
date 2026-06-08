namespace api.Models;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime Deadline { get; set; }
    public bool IsArchived { get; set; } = false;

    // Author: Nicolai and Oliver
    public List<ChatChannel> ChatChannels { get; set; } = new();
    // Author: Nicolai and Oliver
    public List<ProjectTask> Tasks { get; set; } = new();
    // Author: Nicolai and Oliver
    public List<User> Users { get; set; } = new();
}
