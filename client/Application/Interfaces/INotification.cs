namespace client.Application.Interfaces;

public interface INotification
{
  int Id { get; }
  string Text { get; }
  bool IsRead { get; }
  DateTime SentAt { get; }
  // User user { get; }
}
