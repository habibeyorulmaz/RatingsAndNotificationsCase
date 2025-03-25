using System;
using FluentValidation.TestHelper;
using Ratings.Api.DTOs;
using Ratings.Api.Validations;
using Xunit;

namespace Ratings.UnitTests.Validations
{
    public class RatingDtoValidatorTests
    {
        private readonly RatingDtoValidator _validator;

        public RatingDtoValidatorTests()
        {
            _validator = new RatingDtoValidator();
        }

        [Fact]
        public void ShouldNotHaveError_WhenRatingDtoIsValid()
        {
            // Arrange
            var ratingDto = new RatingDto
            {
                ProviderId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Score = 8,
                Comment = "Great service"
            };

            // Act Assert
            var result = _validator.TestValidate(ratingDto);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void ShouldHaveError_WhenProviderIdIsEmpty()
        {
            // Arrange
            var ratingDto = new RatingDto
            {
                ProviderId = Guid.Empty,
                CustomerId = Guid.NewGuid(),
                Score = 8,
                Comment = "Great service"
            };

            // Act Assert
            var result = _validator.TestValidate(ratingDto);
            result.ShouldHaveValidationErrorFor(x => x.ProviderId)
                .WithErrorMessage("ProviderId cannot be empty.");
        }

        [Fact]
        public void ShouldHaveError_WhenCustomerIdIsEmpty()
        {
            // Arrange
            var ratingDto = new RatingDto
            {
                ProviderId = Guid.NewGuid(),
                CustomerId = Guid.Empty,
                Score = 8,
                Comment = "Great service"
            };

            // Act Assert
            var result = _validator.TestValidate(ratingDto);
            result.ShouldHaveValidationErrorFor(x => x.CustomerId)
                .WithErrorMessage("CustomerId cannot be empty.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        public void ShouldHaveError_WhenScoreIsOutOfRange(int score)
        {
            // Arrange
            var ratingDto = new RatingDto
            {
                ProviderId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Score = score,
                Comment = "Great service"
            };

            // Act Assert
            var result = _validator.TestValidate(ratingDto);
            result.ShouldHaveValidationErrorFor(x => x.Score);
        }

        [Theory]
        [InlineData("a")]  // Too short
        [InlineData("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. Lorem ipsum dolor sit amet, consectetur adipiscing elit.")] // Too long
        [InlineData("Invalid@#$%^&*()_+")]  // Invalid characters
        public void ShouldHaveError_WhenCommentIsInvalid(string comment)
        {
            // Arrange
            var ratingDto = new RatingDto
            {
                ProviderId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Score = 8,
                Comment = comment
            };

            // Act Assert
            var result = _validator.TestValidate(ratingDto);
            result.ShouldHaveValidationErrorFor(x => x.Comment);
        }
    }
}