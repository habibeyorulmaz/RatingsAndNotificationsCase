using MassTransit;
using Ratings.Api.Data.Entities;
using Ratings.Api.DTOs;
using Ratings.Api.Repositories;
using SharedLibrary.Messaging;

namespace Ratings.Api.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    private readonly ILogger<RatingService> _logger;

    public RatingService(
    IRatingRepository ratingRepository,
    IPublishEndpoint publishEndpoint,
    ILogger<RatingService> logger)
    {
        _ratingRepository = ratingRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task AddRatingAsync(RatingDto ratingDto, CancellationToken cancellationToken)
    {
        try
        {
            var rating = new Rating
            {
                ProviderId = ratingDto.ProviderId,
                CustomerId = ratingDto.CustomerId,
                Score = ratingDto.Score,
                Comment = ratingDto.Comment
            };

            await _ratingRepository.AddAsync(rating, cancellationToken);
            _logger.LogInformation($"New rating added: Provider {rating.ProviderId}, Score {rating.Score}");
            var message = new RatingCreatedMessage
            {
                ProviderId = rating.ProviderId,
                CustomerId = rating.CustomerId,
                Score = rating.Score,
                Comment = rating.Comment,
                CorrelationId = Guid.NewGuid(),
            };
            // Yeni eklenen puan için RabbitMQ'ya bildirim eventi gönder
            await _publishEndpoint.Publish(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding a rating");
            throw;
        }
    }

    public async Task<double> GetAverageRatingAsync(Guid providerId, CancellationToken cancellationToken)
    {
        return await _ratingRepository.GetAverageRatingAsync(providerId, cancellationToken);
    }
}