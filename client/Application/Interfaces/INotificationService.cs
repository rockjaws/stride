// =============================================================================
// Author: Nicolai and Oliver
// =============================================================================

using client.Domain.Models;

namespace client.Application.Interfaces;

public interface INotificationService
{
    // Author: Oliver
    Task<List<Notification>> GetNotificationsAsync(int userId);
    // Author: Oliver
    Task MarkAsReadAsync(int userId, int notificationId);
    // Author: Nicolai
    Task<List<Notification>> GetDashboardFeedAsync(int userId);
    // Author: Nicolai
    Task<List<Notification>> GetProjectFeedAsync(int projectId, int userId);
    event EventHandler NotificationsChanged;
}
