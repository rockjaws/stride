using api.DTOs;
using api.Models;

namespace api.Extensions;

public static class NotificationExtensions
{
    public static NotificationDto ToDto(this Notification n)
    {
        return new NotificationDto
        {
            Id = n.Id,
            Text = n.Text,
            IsRead = n.IsRead,
            Time = n.Time,
            TaskId = n.TaskId,
            ProjectId = n.ProjectId
        };
    }
}
