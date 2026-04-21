using StayEaseApp.Application.Interfaces;
using StayEaseApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Application.Services;
public class BookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPropertyRepository _propertyRepository;

    public BookingService(IBookingRepository bookingRepository, IPropertyRepository propertyRepository)
    {
        _bookingRepository = bookingRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<Booking> CreateBookingAsync(Guid propertyId, Guid userId, DateTime startDate, DateTime endDate)
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

        return booking;
    }
}
