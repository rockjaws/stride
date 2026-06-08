namespace api.Models;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime Deadline { get; set; }
    public bool IsArchived { get; set; } = false;

    public List<ChatChannel> ChatChannels { get; set; } = new();
    public List<ProjectTask> Tasks { get; set; } = new();
    public List<User> Users { get; set; } = new();
}
