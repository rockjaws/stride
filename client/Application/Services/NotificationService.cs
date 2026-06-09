// =============================================================================
// Author: Nicolaj and Oliver
// =============================================================================

using System.Net.Http;
using System.Net.Http.Json;

using client.Application.Interfaces;
using client.Domain.Models;

namespace client.Application.Services;

public class NotificationService : INotificationService
{
    private readonly HttpClient _httpClient;

    // The last successful poll is retained so unchanged responses do not refresh entire views.
    private HashSet<int> _knownNotificationIds = [];
    private bool _hasNotificationSnapshot;

    public event EventHandler? NotificationsChanged;

    // Author: Nicolaj and Oliver
    public NotificationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Author: Nicolaj and Oliver
    // Notifies view models that feed data may need reloading after the polled set changes.
    private void NotifyNotificationsChanged() => NotificationsChanged?.Invoke(this, EventArgs.Empty);

    // Author: Nicolaj and Oliver
    // Polls the active user's toast notifications and detects additions or removals by id.
    public async Task<List<Notification>> GetNotificationsAsync(int userId)
    {
        var notificationDtos =
          await _httpClient.GetFromJsonAsync<List<NotificationDto>>($"api/users/{userId}/notifications") ?? [];

        var notifications = notificationDtos.Select(ToNotification).ToList();
        var currentNotificationIds = notifications.Select(notification => notification.Id).ToHashSet();

        // The first response establishes a baseline and must not trigger a startup refresh.
        bool notificationsChanged =
            _hasNotificationSnapshot && !_knownNotificationIds.SetEquals(currentNotificationIds);

        // Replace the snapshot only after a successful response has been mapped.
        _knownNotificationIds = currentNotificationIds;
        _hasNotificationSnapshot = true;

        if (notificationsChanged)
            NotifyNotificationsChanged();

        return notifications;
    }

    // Author: Nicolaj and Oliver
    // Persists toast acknowledgement without changing the notification's identity.
    public async Task MarkAsReadAsync(int userId, int notificationId)
    {
        // Marking read is scoped by user so one user's toast state does not hide another user's notification.
        var response = await _httpClient.PutAsJsonAsync(
          $"api/users/{userId}/notifications/{notificationId}",
          new NotificationUpdateDto { IsRead = true }
        );
        response.EnsureSuccessStatusCode();
    }



    // Author: Nicolaj and Oliver
    // Converts the transport shape into the immutable notification model used by the UI.
    private static Notification ToNotification(NotificationDto dto)
    {
        // The client does not need the full related task yet, only the optional task id.
        return new Notification(
          dto.Id,
          dto.Text,
          dto.IsRead,
          dto.Time,
          dto.ProjectId,
          dto.TaskId
        );
    }

    // Author: Nicolaj and Oliver
    // Loads the recent activity feed spanning every project associated with the user.
    public async Task<List<Notification>> GetDashboardFeedAsync(int userId)
    {
        var dtos = await _httpClient.GetFromJsonAsync<List<NotificationDto>>($"api/users/{userId}/project-feeds") ?? [];
        return [.. dtos.Select(ToNotification)];
    }

    // Author: Nicolaj and Oliver
    // Loads activity for one project while keeping entries scoped to the active user.
    public async Task<List<Notification>> GetProjectFeedAsync(int projectId, int userId)
    {
        var dtos = await _httpClient.GetFromJsonAsync<List<NotificationDto>>($"api/projects/{projectId}/notifications?userId={userId}") ?? [];
        return [.. dtos.Select(ToNotification)];
    }

    private sealed class NotificationDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }
    }

    private sealed class NotificationUpdateDto
    {
        public bool IsRead { get; set; }
    }
}
