using MassTransit;
using Notifications.Api.Data.Context;
using Notifications.Api.DTOs;
using Notifications.Api.Services;
using SharedLibrary.Messaging;

namespace Notifications.Api.Consumer;

public class RatingCreatedMessageConsumer : IConsumer<RatingCreatedMessage>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<RatingCreatedMessageConsumer> _logger;

    public RatingCreatedMessageConsumer(AppNotificationsDbContext context, ILogger<RatingCreatedMessageConsumer> logger, INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task Consume(ConsumeContext<RatingCreatedMessage> context)
    {
        var message = context.Message;
        var cancellationToken = context.CancellationToken;

        try
        {
            // Completed your notification here by sending SMS or email
            // ...

            var notification = new NotificationDto
            {
                Comment = message.Comment,
                ProviderId = message.ProviderId,
                CustomerId = message.CustomerId,
                Score = message.Score,
                CorrelationId = message.CorrelationId,
            };

            await _notificationService.AddNotificationAsync(notification, cancellationToken);
            _logger.LogInformation($"New notification was created: Provider {message.ProviderId}, Score {message.Score}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding a notification");
            throw;
        }
        _logger.LogInformation($"RatingCreatedMessage was consumed at {DateTime.UtcNow}. Congratz ! ");

        await Task.CompletedTask;
    }
}