using Ratings.Api.DTOs;

namespace Ratings.Api.Services;

public interface IRatingService
{
    Task AddRatingAsync(RatingDto ratingDto, CancellationToken cancellationToken);
    Task<double> GetAverageRatingAsync(Guid providerId, CancellationToken cancellationToken);
}
