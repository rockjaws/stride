using client.Application.Interfaces;

namespace client.Domain.Models;

public class Notification : INotification
{
  public int Id { get; }
  public string Text { get; }
  public bool IsRead { get; }
  public DateTime SentAt { get; }
  public int? TaskId { get; }
  public IUser? User { get; }

  public Notification(
    int id,
    string text,
    bool isRead,
    DateTime sentAt,
    int? taskId = null,
    IUser? user = null
  )
  {
    Id = id;
    Text = text;
    IsRead = isRead;
    SentAt = sentAt;
    TaskId = taskId;
    User = user;
  }
}
