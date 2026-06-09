// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using client.Domain.Models;

namespace client.Application.Interfaces;

public interface INotificationService
{
    // Author: Oliver
    // Polls notifications for toast handling and detects changes to the notification set.
    Task<List<Notification>> GetNotificationsAsync(int userId);

    // Author: Oliver
    // Marks one notification as acknowledged for the specified user.
    Task MarkAsReadAsync(int userId, int notificationId);

    // Author: Nicolaj
    // Returns recent project activity shown on the dashboard.
    Task<List<Notification>> GetDashboardFeedAsync(int userId);

    // Author: Nicolaj
    // Returns activity for one project and one user's feed.
    Task<List<Notification>> GetProjectFeedAsync(int projectId, int userId);

    // Raised only when polling detects that notification identities were added or removed.
    event EventHandler NotificationsChanged;
}
