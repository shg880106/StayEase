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

    [Fact]
    public async Task Should_Create_Booking_Successfully_When_No_Overlaps()
    {
        // Arrange
        var property = new Property("Test", "Description", 100m, "Location", 4, "url", Guid.NewGuid());

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(_propertyId))
            .ReturnsAsync(property);

        _bookingRepositoryMock
            .Setup(r => r.GetByPropertyIdAsync(_propertyId))
            .ReturnsAsync(new List<Booking>());

        _bookingRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Booking>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _bookingService.CreateBookingAsync(
            _propertyId,
            _userId,
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 15));

        // Assert
        result.Should().NotBeNull();
        result.PropertyID.Should().Be(_propertyId);
        result.UserID.Should().Be(_userId);
        result.TotalPrice.Should().Be(500m); // 5 nights * 100

        _bookingRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Booking>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Property_Not_Found()
    {
        // Arrange
        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(_propertyId))
            .ReturnsAsync((Property?)null);

        // Act
        Func<Task> act = async () =>
            await _bookingService.CreateBookingAsync(
                _propertyId,
                _userId,
                new DateTime(2026, 1, 10),
                new DateTime(2026, 1, 15));

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Property not found");

        _bookingRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Booking>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Not_Create_Booking_When_Multiple_Overlaps_Exist()
    {
        // Arrange
        var existingBookings = new List<Booking>
        {
            CreateBooking(new DateTime(2026, 1, 5), new DateTime(2026, 1, 10)),
            CreateBooking(new DateTime(2026, 1, 12), new DateTime(2026, 1, 15))
        };

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(_propertyId))
            .ReturnsAsync(new Property("Test", "Desc", 100m, "Loc", 4, "url", Guid.NewGuid()));

        _bookingRepositoryMock
            .Setup(r => r.GetByPropertyIdAsync(_propertyId))
            .ReturnsAsync(existingBookings);

        // Act
        Func<Task> act = async () =>
            await _bookingService.CreateBookingAsync(
                _propertyId,
                _userId,
                new DateTime(2026, 1, 8),
                new DateTime(2026, 1, 11));

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Property is already booked for the selected dates");
    }

    [Fact]
    public async Task Should_Allow_Booking_When_Starts_On_Previous_EndDate()
    {
        // Arrange
        var existingBookings = new List<Booking>
        {
            CreateBooking(new DateTime(2026, 1, 10), new DateTime(2026, 1, 15))
        };

        var property = new Property("Test", "Desc", 100m, "Loc", 4, "url", Guid.NewGuid());

        _propertyRepositoryMock
            .Setup(r => r.GetByIdAsync(_propertyId))
            .ReturnsAsync(property);

        _bookingRepositoryMock
            .Setup(r => r.GetByPropertyIdAsync(_propertyId))
            .ReturnsAsync(existingBookings);

        _bookingRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Booking>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _bookingService.CreateBookingAsync(
            _propertyId,
            _userId,
            new DateTime(2026, 1, 15),
            new DateTime(2026, 1, 20));

        // Assert
        result.Should().NotBeNull();
        _bookingRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Booking>()),
            Times.Once);
    }
}
