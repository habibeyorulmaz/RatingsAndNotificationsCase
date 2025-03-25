using Microsoft.EntityFrameworkCore;
using Notifications.Api.Data.Context;
using Notifications.Api.Data.Entities;

namespace Notifications.Api.Repositories;

public class NotificationRepository(AppNotificationsDbContext _appNotificationsDbContext) : INotificationRepository
{
    public Task<List<Notification>> GetNotificationsAsync()
    {
        var notifications = _appNotificationsDbContext.Notifications.AsQueryable();
         return notifications.ToListAsync();
    }
    public async Task AddAsync(Notification notification, CancellationToken cancellationToken)
    {
        await _appNotificationsDbContext.Notifications.AddAsync(notification);
        await _appNotificationsDbContext.SaveChangesAsync();
    }
}
