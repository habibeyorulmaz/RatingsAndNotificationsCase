using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Ratings.Api.Data.Entities;
using Ratings.Api.DTOs;
using Ratings.Api.Repositories;
using Ratings.Api.Services;
using SharedLibrary.Messaging;
using Xunit;

namespace Ratings.UnitTests.Services
{
    public class RatingServiceTests
    {
        private readonly Mock<IRatingRepository> _mockRepository;
        private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
        private readonly Mock<ILogger<RatingService>> _mockLogger;
        private readonly RatingService _ratingService;

        public RatingServiceTests()
        {
            _mockRepository = new Mock<IRatingRepository>();
            _mockPublishEndpoint = new Mock<IPublishEndpoint>();
            _mockLogger = new Mock<ILogger<RatingService>>();

            _ratingService = new RatingService(
                _mockRepository.Object,
                _mockPublishEndpoint.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task AddRatingAsync_ShouldAddRatingToRepository_AndPublishMessage()
        {
            // Arrange
            var ratingDto = new RatingDto
            {
                ProviderId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Score = 8,
                Comment = "Great service"
            };

            // Act
            await _ratingService.AddRatingAsync(ratingDto, CancellationToken.None);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(
                It.Is<Rating>(rating =>
                    rating.ProviderId == ratingDto.ProviderId &&
                    rating.CustomerId == ratingDto.CustomerId &&
                    rating.Score == ratingDto.Score &&
                    rating.Comment == ratingDto.Comment),
                It.IsAny<CancellationToken>()),
                Times.Once);

            _mockPublishEndpoint.Verify(p => p.Publish(
                It.Is<RatingCreatedMessage>(msg =>
                    msg.ProviderId == ratingDto.ProviderId &&
                    msg.CustomerId == ratingDto.CustomerId &&
                    msg.Score == ratingDto.Score &&
                    msg.Comment == ratingDto.Comment),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task AddRatingAsync_WhenRepositoryThrowsException_ShouldLogAndRethrowException()
        {
            // Arrange
            var ratingDto = new RatingDto
            {
                ProviderId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Score = 8,
                Comment = "Great service"
            };

            var expectedException = new Exception("Test exception");
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Rating>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _ratingService.AddRatingAsync(ratingDto, CancellationToken.None));

            Assert.Same(expectedException, exception);
            _mockPublishEndpoint.Verify(p => p.Publish(It.IsAny<RatingCreatedMessage>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAverageRatingAsync_ShouldReturnAverageFromRepository()
        {
            // Arrange
            var providerId = Guid.NewGuid();
            var expectedAverage = 4.5;

            _mockRepository.Setup(r => r.GetAverageRatingAsync(providerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAverage);

            // Act
            var result = await _ratingService.GetAverageRatingAsync(providerId, CancellationToken.None);

            // Assert
            Assert.Equal(expectedAverage, result);
            _mockRepository.Verify(r => r.GetAverageRatingAsync(providerId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
