// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using client.Domain.Models;

namespace client.Application.Interfaces;

public interface INotificationService
{
    // Author: Oliver
    Task<List<Notification>> GetNotificationsAsync(int userId);
    // Author: Oliver
    Task MarkAsReadAsync(int userId, int notificationId);
    // Author: Nicolaj
    Task<List<Notification>> GetDashboardFeedAsync(int userId);
    // Author: Nicolaj
    Task<List<Notification>> GetProjectFeedAsync(int projectId, int userId);
    event EventHandler NotificationsChanged;
}
