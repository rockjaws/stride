// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

namespace client.Application.Interfaces;

public interface INotification
{
  int Id { get; }
  string Text { get; }
  bool IsRead { get; }
  DateTime SentAt { get; }
  int? TaskId { get; }
  IUser? User { get; }
}
