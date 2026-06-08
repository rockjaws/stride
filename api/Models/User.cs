namespace api.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string WorkMail { get; set; } = string.Empty;

    // Author: Nicolai and Oliver
    public List<Project> Projects { get; set; } = new();
    // Author: Nicolai and Oliver
    public List<ProjectTask> ProjectTasks { get; set; } = new();
}
