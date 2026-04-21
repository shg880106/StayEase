using Microsoft.AspNetCore.Mvc;
using StayEaseApp.Application.DTOs;
using StayEaseApp.Application.Services;

namespace StayEaseApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly BookingService _bookingService;

    public BookingController(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Creates a new booking
    /// </summary>
    /// <param name="request">The booking request data.</param>
    /// <returns>
    /// Returns one of the following HTTP status codes:
    /// <list type="bullet">
    ///   <item><description>201 Created - Booking successfully created with booking details in response body</description></item>
    ///   <item><description>400 Bad Request - Invalid input data or booking validation failed</description></item>
    ///   <item><description>404 Not Found - Property does not exist or is unavailable</description></item>
    /// </list>
    /// </returns>
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateBooking(CreateBookingRequestDto request)
    {
        try
        {
            var booking = await _bookingService.CreateBookingAsync(
                request.PropertyID,
                request.UserID,
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
