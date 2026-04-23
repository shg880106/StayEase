using FluentAssertions;
using Moq;
using StayEaseApp.Application.Interfaces;
using StayEaseApp.Application.Services;
using StayEaseApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Tests.Application;
public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
    private readonly BookingService _bookingService;
    private readonly Guid _propertyId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    public BookingServiceTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _propertyRepositoryMock = new Mock<IPropertyRepository>();
        _bookingService = new BookingService(_bookingRepositoryMock.Object, _propertyRepositoryMock.Object);
    }

    private Booking CreateBooking(DateTime start, DateTime end)
    {
        return new Booking(_propertyId, _userId, start, end, 100);
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Booking_Overlaps()
    {
        // Arrange (mock repository with existing booking)
        var existingBookings = new List<Booking>
        {
            CreateBooking(
                new DateTime(2026, 1, 10),
                new DateTime(2026, 1, 15))
        };

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(_propertyId))
            .ReturnsAsync(new Property("Test Title", "Test Description", 100, "Test Location", 4, "Test ImageUrl", _userId));

        _bookingRepositoryMock
            .Setup(r => r.GetByPropertyIdAsync(_propertyId))
            .ReturnsAsync(existingBookings);

        // Act
        Func<Task> act = async () =>
           await _bookingService.CreateBookingAsync(
               _propertyId,
               _userId,
               new DateTime(2026, 1, 12),
               new DateTime(2026, 1, 18));

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Property is already booked for the selected dates");
    }
}
