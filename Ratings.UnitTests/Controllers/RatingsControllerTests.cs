using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Ratings.Api.Controllers;
using Ratings.Api.DTOs;
using Ratings.Api.Services;
using Xunit;

namespace Ratings.UnitTests.Controllers
{
    public class RatingsControllerTests
    {
        private readonly Mock<IRatingService> _mockService;
        private readonly RatingsController _controller;

        public RatingsControllerTests()
        {
            _mockService = new Mock<IRatingService>();
            _controller = new RatingsController(_mockService.Object);
        }

        [Fact]
        public async Task AddRating_ShouldReturnOk_WhenServiceSucceeds()
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
            var result = await _controller.AddRating(ratingDto, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            _mockService.Verify(s => s.AddRatingAsync(ratingDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetAverageRating_ShouldReturnOkWithCorrectValue()
        {
            // Arrange
            var providerId = Guid.NewGuid();
            var expectedAverage = 4.5;

            _mockService.Setup(s => s.GetAverageRatingAsync(providerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAverage);

            // Act
            var result = await _controller.GetAverageRating(providerId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedAverage, okResult.Value);
            _mockService.Verify(s => s.GetAverageRatingAsync(providerId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}