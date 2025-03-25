using Notifications.Api.Data.Entities;

namespace Notifications.Api.Repositories;

public interface INotificationRepository
{
    Task<List<Notification>> GetNotificationsAsync();

    Task AddAsync(Notification notification, CancellationToken cancellationToken);
}