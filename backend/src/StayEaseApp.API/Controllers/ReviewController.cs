using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEaseApp.Application.DTOs;
using StayEaseApp.Application.Interfaces;
using StayEaseApp.Application.Services;
using System.Security.Claims;

namespace StayEaseApp.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    /// <summary>
    /// Get the review details by the review id
    /// </summary>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>200 Ok - Review details in response body</description></item>
    ///   <item><description>404 Not Found - Review not found</description></item>
    /// </list>
    /// </returns>
    [HttpGet("{reviewId}")]
    [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReviewById(Guid reviewId)
    {
        try
        {
            var review = await _reviewService.GetReviewByIdAsync(reviewId);
            if (review == null)
            {
                return NotFound($"Review with ID {reviewId} not found.");
            }
            return Ok(review);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create a new review (requires authentication)
    /// </summary>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>201 Created - Review created successfully</description></item>
    ///   <item><description>400 Bad Request - Invalid input data or review validation failed</description></item>
    ///   <item><description>401 Unauthorized - User not authenticated</description></item>
    /// </list>
    /// </returns>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ReviewResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequestDto reviewRequest)
    {
        try
        {
            // Extract UserID from JWT token claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user authentication" });
            }

            // Override the UserID with the authenticated user's ID
            reviewRequest.UserID = userId;

            var createdReview = await _reviewService.CreateReviewAsync(reviewRequest);

            return CreatedAtAction(nameof(GetReviewById), new { reviewId = createdReview.ReviewID }, createdReview);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
