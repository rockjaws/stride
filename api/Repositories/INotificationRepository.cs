using api.Models;

namespace api.Repositories;

public interface INotificationRepository
{
  Task<IEnumerable<Notification>> GetNotificationsByIdAsync(int id);
  Task<Notification?> GetNotificationByIdAsync(int id);
  Task AddNotificationAsync(Notification notification);
  Task UpdateNotification(Notification notification);
  Task SaveChangesAsync();
}
