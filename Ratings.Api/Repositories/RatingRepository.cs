using Microsoft.EntityFrameworkCore;
using Ratings.Api.Data.Context;
using Ratings.Api.Data.Entities;

namespace Ratings.Api.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly AppRatingsDbContext _context;

    public RatingRepository(AppRatingsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Rating rating, CancellationToken cancellationToken)
    {
        await _context.Ratings.AddAsync(rating);
        await _context.SaveChangesAsync();
    }

    public async Task<double> GetAverageRatingAsync(Guid providerId, CancellationToken cancellationToken)
    {
        return await _context.Ratings
            .Where(r => r.ProviderId == providerId)
            .AverageAsync(r => r.Score);
    }
}