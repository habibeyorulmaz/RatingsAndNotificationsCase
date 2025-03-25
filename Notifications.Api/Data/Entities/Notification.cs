using SharedLibrary.Abstract.Entity;

namespace Notifications.Api.Data.Entities;

public class Notification : BaseEntity
{
    public Guid ProviderId { get; set; } // Id of person gives service
    public Guid CustomerId { get; set; } // Id of person takes service
    public int Score { get; set; } // Point from 1 to 10
    public string? Comment { get; set; } // Comment of customer
    public bool IsSent { get; set; } = false; // Notification send 
    public Guid CorrelationId { get; set; } // Id for using request/response/message/event/db tracing (a.k.a TraceId)
}