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
      .Where(u => u.UserId == id)
      .ToListAsync();
  }

  public async Task<Notification?> GetNotificationByIdAsync(int id)
  {
    return await _context.Notifications.FindAsync(id);
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
