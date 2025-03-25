using Notifications.Api.Data.Entities;
using Notifications.Api.DTOs;
using Notifications.Api.Repositories;

namespace Notifications.Api.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationRepository notificationRepository, ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task<List<Notification>> GetNotificationsAsync()
    {
        return await _notificationRepository.GetNotificationsAsync();
    }

    public async Task AddNotificationAsync(NotificationDto notificationDto, CancellationToken cancellationToken)
    {
        try
        {
            var notification = new Notification
            {
                ProviderId = notificationDto.ProviderId,
                CustomerId = notificationDto.CustomerId,
                Score = notificationDto.Score,
                Comment = notificationDto.Comment,
                CorrelationId = notificationDto.CorrelationId,
            };

            await _notificationRepository.AddAsync(notification, cancellationToken);

            _logger.LogInformation($"New notification added: Provider {notification.ProviderId}, Score {notification.Score}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding a notification");
            throw;
        }
    }
}