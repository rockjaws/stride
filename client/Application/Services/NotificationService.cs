using System.Net.Http;
using System.Net.Http.Json;
using client.Application.Interfaces;
using client.Domain.Models;

namespace client.Application.Services;

public class NotificationService : INotificationService
{
  private readonly HttpClient _httpClient;

  public NotificationService()
  {
    _httpClient = new HttpClient
    {
      BaseAddress = new Uri("http://localhost:5189")
    };
  }

  public async Task<List<Notification>> GetNotificationsAsync(int userId)
  {
    var notificationDtos =
      await _httpClient.GetFromJsonAsync<List<NotificationDto>>($"api/users/{userId}/notifications") ?? [];

    return [.. notificationDtos.Select(ToNotification)];
  }

  public async Task MarkAsReadAsync(int userId, int notificationId)
  {
    // Marking read is scoped by user so one user's toast state does not hide another user's notification.
    var response = await _httpClient.PutAsJsonAsync(
      $"api/users/{userId}/notifications/{notificationId}",
      new NotificationUpdateDto { IsRead = true }
    );
    response.EnsureSuccessStatusCode();
  }

  private static Notification ToNotification(NotificationDto dto)
  {
    return new Notification(
      dto.Id,
      dto.Text,
      dto.IsRead,
      dto.Time,
      dto.TaskId
    );
  }

  private sealed class NotificationDto
  {
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime Time { get; set; } = DateTime.Now;
    public int? TaskId { get; set; }
  }

  private sealed class NotificationUpdateDto
  {
    public bool IsRead { get; set; }
  }
}
