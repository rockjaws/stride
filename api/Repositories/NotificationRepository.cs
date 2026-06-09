using api.Data;
using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByIdAsync(int id)
    {
        return await _context.Notifications
          // Clients poll by active user id and then mark individual notifications as read.
          .Where(u => u.UserId == id)
          .ToListAsync();
    }

    public async Task<Notification?> GetNotificationByIdAsync(int id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByProjectIdAsync(int projectId)
    {
        return await _context.Notifications
            .Where(n => n.ProjectId == projectId)
            .OrderByDescending(n => n.Time)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByProjectIdsAsync(IEnumerable<int> projectIds)
    {
        return await _context.Notifications
            .Where(n => projectIds.Contains(n.ProjectId))
            .OrderByDescending(n => n.Time)
            // Bound the cross-project dashboard feed so it cannot grow without limit.
            .Take(50)
            .ToListAsync();
    }

    public async Task AddNotificationAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public async Task UpdateNotification(Notification notification)
    {
        _context.Notifications.Update(notification);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
