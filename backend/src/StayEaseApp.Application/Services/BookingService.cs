using StayEaseApp.Application.DTOs;
using StayEaseApp.Application.Interfaces;
using StayEaseApp.Application.Mappers;
using StayEaseApp.Domain.Entities;
using StayEaseApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.Services;
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly BookingMapper _mapper = new();

    public BookingService(IBookingRepository bookingRepository, IPropertyRepository propertyRepository)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<BookingResponseDto> CancelBookingAsync(Guid bookingId, Guid userId)
    {
        // 1. Get booking
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            throw new Exception("Booking not found");

        // 2. Verify booking belongs to the user
        if (booking.UserID != userId)
            throw new UnauthorizedAccessException("You don't have permission to cancel this booking");

        // 3. Verify booking is in  Pending status (not Confirmed or already Cancelled)
        if (booking.BookingStatus == Status.Confirmed)
            throw new InvalidOperationException("Cannot cancel a confirmed booking");

        if (booking.BookingStatus == Status.Cancelled)
            throw new InvalidOperationException("Booking is already cancelled");

        if (booking.BookingStatus != Status.Pending)
            throw new InvalidOperationException("Only pending bookings can be cancelled");

        // 4. Update booking status to Cancelled
        booking.BookingStatus = Status.Cancelled;

        // 5. Save changes
        await _bookingRepository.UpdateAsync(booking);

        // Map to DTO
        return _mapper.BookingToBookingResponseDto(booking);
    }

    public async Task<BookingResponseDto> CreateBookingAsync(Guid propertyId, Guid userId, DateTime startDate, DateTime endDate)
    {
        // 1. Get property (needed for price)
        var property = await _propertyRepository.GetByIdAsync(propertyId);

        if (property == null)
            throw new Exception("Property not found");

        // 2. Check overlapping bookings
        var existingBookings = await _bookingRepository.GetByPropertyIdAsync(propertyId);

        if (existingBookings.Any(b => b.Overlaps(startDate, endDate)))
            throw new Exception("Property is already booked for the selected dates");

        // 3. Create booking (domain logic calculates price)
        var booking = new Booking(propertyId, userId, startDate, endDate, property.PricePerNight);

        // 4. Save booking
        await _bookingRepository.AddAsync(booking);

        // Map to DTO        
        return _mapper.BookingToBookingResponseDto(booking);
    }

    public async Task<BookingDetailsDto?> GetBookingDetailsAsync(Guid bookingId, Guid userId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            return null;

        // Ensure the booking belongs to the requesting user
        if (booking.UserID != userId)
            throw new UnauthorizedAccessException("You don't have permission to view this booking");

        // Map to detailed DTO
        return new BookingDetailsDto
        {
            BookingID = booking.BookingID,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            TotalPrice = booking.TotalPrice,
            BookingStatus = booking.BookingStatus,
            Property = new PropertyDetailsDto
            {
                PropertyID = booking.Property.PropertyID,
                Title = booking.Property.Title,
                Location = booking.Property.Location,
                Description = booking.Property.Description,
                PricePerNight = booking.Property.PricePerNight,
                ImageUrl = booking.Property.ImageUrl
            },
            Owner = new OwnerDetailsDto
            {
                Name = booking.Property.Owner.Name,
                Email = booking.Property.Owner.Email
            }
        };
    }

    public async Task<List<BookingResponseDto>> GetUserBookingsAsync(Guid userId)
    {
        var bookings = await _bookingRepository.GetByUserIdAsync(userId);
        return bookings.Select(b => _mapper.BookingToBookingResponseDto(b)).ToList();
    }
}
