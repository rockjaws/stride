using api.Data;
using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    // Author: Nicolai and Oliver
    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    // Author: Nicolai and Oliver
    public async Task<IEnumerable<Notification>> GetNotificationsByIdAsync(int id)
    {
        return await _context.Notifications
          // Clients poll by active user id and then mark individual notifications as read.
          .Where(u => u.UserId == id)
          .ToListAsync();
    }

    // Author: Nicolai and Oliver
    public async Task<Notification?> GetNotificationByIdAsync(int id)
    {
        return await _context.Notifications.FindAsync(id);
    }

    // Author: Nicolai and Oliver
    public async Task<IEnumerable<Notification>> GetNotificationsByProjectIdAsync(int projectId)
    {
        return await _context.Notifications
            .Where(n => n.ProjectId == projectId)
            .OrderByDescending(n => n.Time)
            .ToListAsync();
    }

    // Author: Nicolai and Oliver
    public async Task<IEnumerable<Notification>> GetNotificationsByProjectIdsAsync(IEnumerable<int> projectIds)
    {
        return await _context.Notifications
            .Where(n => projectIds.Contains(n.ProjectId))
            .OrderByDescending(n => n.Time)
            .Take(50)
            .ToListAsync();
    }

    // Author: Nicolai and Oliver
    public async Task AddNotificationAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    // Author: Nicolai and Oliver
    public async Task UpdateNotification(Notification notification)
    {
        _context.Notifications.Update(notification);
        await Task.CompletedTask;
    }

    // Author: Nicolai and Oliver
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
