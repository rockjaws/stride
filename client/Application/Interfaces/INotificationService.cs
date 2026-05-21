using client.Domain.Models;

namespace client.Application.Interfaces;

public interface INotificationService
{
    Task<List<Notification>> GetNotificationsAsync(int userId);
    Task MarkAsReadAsync(int userId, int notificationId);
}
