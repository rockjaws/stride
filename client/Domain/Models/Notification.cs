using client.Application.Interfaces;

namespace client.Domain.Models;

public class Notification : INotification
{
  public int Id { get; }
  public string Text { get; }
  public bool IsRead { get; }
  public DateTime SentAt { get; }
  public IUser User { get; }
}
