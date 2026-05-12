using client.Domain.Enum;

namespace client.Application.Interfaces;

public interface ITask
{
  int Id { get; }
  string Title { get; }
  string Description { get; }
  DateTime StartDate { get; }
  DateTime Deadline { get; }
  TaskProgress Progress { get; }
  TaskPriority Priority { get; }
}
