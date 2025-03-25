namespace Notifications.Api.DTOs;

public class NotificationDto
{
    public Guid ProviderId { get; set; } // Id of person gives service
    public Guid CustomerId { get; set; } // Id of person takes service
    public int Score { get; set; } // Point from 1 to 10
    public string? Comment { get; set; } // Comment of customer
    public Guid CorrelationId { get; set; } // Id for using request/response/message/event/db tracing (a.k.a TraceId)
}