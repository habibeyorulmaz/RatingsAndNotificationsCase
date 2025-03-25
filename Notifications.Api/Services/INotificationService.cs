using Notifications.Api.Data.Entities;
using Notifications.Api.DTOs;

namespace Notifications.Api.Services;

public interface INotificationService
{
    Task<List<Notification>> GetNotificationsAsync();

    Task AddNotificationAsync(NotificationDto notificationDto, CancellationToken cancellationToken);
}