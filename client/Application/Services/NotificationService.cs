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

    public event EventHandler? NotificationsChanged;

    // Author: Nicolaj and Oliver
    public NotificationService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5189")
        };
    }

    // Author: Nicolaj and Oliver
    private void NotifyNotificationsChanged() => NotificationsChanged?.Invoke(this, EventArgs.Empty);

    // Author: Nicolaj and Oliver
    public async Task<List<Notification>> GetNotificationsAsync(int userId)
    {
        var notificationDtos =
          await _httpClient.GetFromJsonAsync<List<NotificationDto>>($"api/users/{userId}/notifications") ?? [];

        var notifications = notificationDtos.Select(ToNotification).ToList();

        if (notifications.Count != 0) NotifyNotificationsChanged();

        return notifications;
    }

    // Author: Nicolaj and Oliver
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
    public async Task<List<Notification>> GetDashboardFeedAsync(int userId)
    {
        var dtos = await _httpClient.GetFromJsonAsync<List<NotificationDto>>($"api/users/{userId}/project-feeds") ?? [];
        return [.. dtos.Select(ToNotification)];
    }

    // Author: Nicolaj and Oliver
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
