using Ratings.Api.Data.Entities;

namespace Ratings.Api.Repositories;

public interface IRatingRepository
{
    Task AddAsync(Rating rating, CancellationToken cancellationToken);
    Task<double> GetAverageRatingAsync(Guid providerId, CancellationToken cancellationToken);
}
