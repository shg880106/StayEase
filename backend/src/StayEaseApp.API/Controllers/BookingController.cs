using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayEaseApp.Application.DTOs;
using StayEaseApp.Application.Interfaces;
using StayEaseApp.Application.Services;
using System.Security.Claims;

namespace StayEaseApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Creates a new booking for the authenticated user
    /// </summary>
    /// <param name="request">The booking request data (PropertyID, StartDate, EndDate)</param>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>200 OK - Booking successfully created with booking details in response body</description></item>
    ///   <item><description>400 Bad Request - Invalid input data or booking validation failed</description></item>
    ///   <item><description>401 Unauthorized - User not authenticated</description></item>
    ///   <item><description>404 Not Found - Property does not exist or is unavailable</description></item>
    /// </list>
    /// </returns>
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequestDto request)
    {
        try
        {
            // Extract UserID from JWT token claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user authentication" });
            }

            // Create booking with authenticated user's ID
            var booking = await _bookingService.CreateBookingAsync(
                request.PropertyID,
                userId,  // Use UserID from JWT token
                request.StartDate,
                request.EndDate); 

            var response = new BookingResponseDto
            {
                BookingID = booking.BookingID,
                TotalPrice = booking.TotalPrice
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
