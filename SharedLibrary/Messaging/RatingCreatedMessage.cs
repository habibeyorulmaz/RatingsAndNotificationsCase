namespace SharedLibrary.Messaging;

public class RatingCreatedMessage
{
    public Guid ProviderId { get; set; }
    public Guid CustomerId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public Guid CorrelationId { get; set; }
}