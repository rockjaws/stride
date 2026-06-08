// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

using client.Application.Interfaces;

namespace client.Domain.Models;

public class Project : IProject
{
    public int? Id { get; }
    public string Title { get; }
    public string Description { get; }
    public bool IsArchived { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime Deadline { get; private set; }
    public List<IChatChannel> ChatChannels { get; private set; }
    public List<ITask> Tasks { get; private set; }

    public List<User> Members { get; private set; }

    // Author: Nicolai and Oliver
    public Project(
        int? id,
        string title,
        string description,
        DateTime startDate,
        DateTime deadline,
        List<IChatChannel>? chatChannels,
        bool isArchived = false,
        List<ITask>? tasks = null,
        List<User>? members = null
    )
    {
        Id = id;
        Title = title;
        Description = description;
        IsArchived = isArchived;
        StartDate = startDate;
        Deadline = deadline;
        ChatChannels = chatChannels ?? [];
        Tasks = tasks ?? [];
        Members = members ?? [];
    }

    // Author: Oliver
    public void Archive()
    {
        IsArchived = true;
    }

    // Author: Oliver
    public void UnArchive()
    {
        IsArchived = false;
    }
}
