using api.Models;

namespace api.Repositories;

public interface INotificationRepository
{
    Task<IEnumerable<Notification>> GetNotificationsByIdAsync(int id);
    Task<Notification?> GetNotificationByIdAsync(int id);
    Task<IEnumerable<Notification>> GetNotificationsByProjectIdAsync(int projectId);
    Task<IEnumerable<Notification>> GetNotificationsByProjectIdsAsync(IEnumerable<int> projectIds);
    Task AddNotificationAsync(Notification notification);
    Task UpdateNotification(Notification notification);
    Task SaveChangesAsync();
}
