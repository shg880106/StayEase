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
            var response = await _bookingService.CreateBookingAsync(
                request.PropertyID,
                userId,  
                request.StartDate,
                request.EndDate); 

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get all bookings owned by the authenticated user
    /// </summary>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>200 OK - List of user's bookings (can be empty)</description></item>
    ///   <item><description>401 Unauthorized - User not authenticated</description></item>
    /// </list>
    /// </returns>
    [HttpGet("my-bookings")]
    [ProducesResponseType(typeof(List<BookingResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyBookings()
    {
        try
        {
            // Extract UserID from JWT token claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user authentication" });
            }

            var bookings = await _bookingService.GetUserBookingsAsync(userId);

            return Ok(bookings);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get detailed information about a specific booking
    /// </summary>
    /// <param name="bookingId">The ID of the booking to retrieve</param>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>200 OK - Booking details including property and owner information</description></item>
    ///   <item><description>401 Unauthorized - User not authenticated or not authorized to view this booking</description></item>
    ///   <item><description>404 Not Found - Booking does not exist</description></item>
    /// </list>
    /// </returns>
    [HttpGet("{bookingId}")]
    [ProducesResponseType(typeof(BookingDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookingDetails(Guid bookingId)
    {
        try
        {
            // Extract UserID from JWT token claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user authentication" });
            }

            var bookingDetails = await _bookingService.GetBookingDetailsAsync(bookingId, userId);

            if (bookingDetails == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            return Ok(bookingDetails);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cancel a booking that belongs to the authenticated user
    /// </summary>
    /// <param name="bookingId">The ID of the booking to cancel</param>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>200 OK - Booking successfully cancelled</description></item>
    ///   <item><description>400 Bad Request - Booking cannot be cancelled (already confirmed or cancelled, or invalid status)</description></item>
    ///   <item><description>401 Unauthorized - User not authenticated or not authorized to cancel this booking</description></item>
    ///   <item><description>404 Not Found - Booking does not exist</description></item>
    /// </list>
    /// </returns>
    [HttpPatch("{bookingId}/cancel")]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelBooking(Guid bookingId)
    {
        try
        {
            // Extract UserID from JWT token claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid or missing user authentication" });
            }

            var response = await _bookingService.CancelBookingAsync(bookingId, userId);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            if (ex.Message == "Booking not found")
            {
                return NotFound(new { message = ex.Message });
            }
            return BadRequest(new { message = ex.Message });
        }
    }
}
