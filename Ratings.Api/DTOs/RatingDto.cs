namespace Ratings.Api.DTOs;

public class RatingDto
{
    public Guid ProviderId { get; set; } // Id of person gives service
    public Guid CustomerId { get; set; } // Id of person takes service
    public int Score { get; set; } // Point from 1 to 10
    public string? Comment { get; set; } // Comment of customer
}
