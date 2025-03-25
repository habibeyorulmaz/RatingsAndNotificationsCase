using Microsoft.AspNetCore.Mvc;
using Ratings.Api.DTOs;
using Ratings.Api.Services;
using SharedLibrary.Middlewares.GlobalExceptionHandler;

namespace Ratings.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpPost]
    public async Task<IActionResult> AddRating([FromBody] RatingDto ratingDto, CancellationToken cancellationToken)
    {
        await _ratingService.AddRatingAsync(ratingDto, cancellationToken);
        return Ok();
    }

    [HttpGet("{providerId}/average")]
    public async Task<IActionResult> GetAverageRating(Guid providerId, CancellationToken cancellationToken)
    {
        var average = await _ratingService.GetAverageRatingAsync(providerId, cancellationToken);
        return Ok(average);
    }
}