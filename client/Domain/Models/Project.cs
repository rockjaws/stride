using client.Application.Interfaces;

namespace client.Domain.Models;

public class Project : IProject
{
  public int? Id { get; }
  public string Title { get; }
  public string Description { get; }
  public DateTime StartDate { get; private set; }
  public DateTime Deadline { get; private set; }
  public List<IChatChannel> ChatChannels { get; private set; }
  public List<ITask> Tasks { get; private set; }

  public Project(
      int? id,
      string title,
      string description,
      DateTime startDate,
      DateTime deadline,
      List<IChatChannel>? chatChannels,
      List<ITask>? tasks = null
  )
  {
    Id = id;
    Title = title;
    Description = description;
    StartDate = startDate;
    Deadline = deadline;
    ChatChannels = chatChannels ?? [];
    Tasks = tasks ?? [];
  }
}
