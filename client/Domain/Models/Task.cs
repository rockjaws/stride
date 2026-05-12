using client.Application.Interfaces;
using client.Domain.Enum;

namespace client.Domain.Models;

public class Task : ITask
{
  public int Id { get; }
  public string Title { get; }
  public string Description { get; }
  public DateTime StartDate { get; }
  public DateTime Deadline { get; }
  public TaskProgress Progress { get; }
  public TaskPriority Priority { get; }
}
