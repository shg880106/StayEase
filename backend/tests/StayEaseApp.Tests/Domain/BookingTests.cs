using FluentAssertions;
using StayEaseApp.Domain.Entities;
using StayEaseApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.Tests.Domain;
public class BookingTests
{
    private readonly Guid _propertyId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    private Booking CreateBooking(DateTime starDate, DateTime endDate)
    {
        return new Booking(_propertyId, _userId, starDate, endDate, 100);
    }

    [Fact]
    public void Should_Return_True_When_Bookings_Overlap()
    {
        // Arrange
        var existing = CreateBooking(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 15));

        // Act
        var overlaps = existing.Overlaps(
            new DateTime(2026, 1, 12),
            new DateTime(2026, 1, 18));

        // Assert
        overlaps.Should().BeTrue();
    }

    [Fact]
    public void Should_Return_False_When_Bookings_Do_Not_Overlap()
    {
        // Arrange
        var existing = CreateBooking(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 15));

        // Act
        var overlaps = existing.Overlaps(
            new DateTime(2026, 1, 16),
            new DateTime(2026, 1, 20));

        // Assert
        overlaps.Should().BeFalse();
    }

    [Fact]
    public void Should_Return_True_When_Booking_Starts_Inside_Existing()
    {
        var existing = CreateBooking(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 15));

        var overlaps = existing.Overlaps(
            new DateTime(2026, 1, 14),
            new DateTime(2026, 1, 18));

        overlaps.Should().BeTrue();
    }

    [Fact]
    public void Should_Return_True_When_Booking_Ends_Inside_Existing()
    {
        var existing = CreateBooking(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 15));

        var overlaps = existing.Overlaps(
            new DateTime(2026, 1, 5),
            new DateTime(2026, 1, 12));

        overlaps.Should().BeTrue();
    }

    [Fact]
    public void Should_Return_True_When_Booking_Fully_Encloses_Existing()
    {
        var existing = CreateBooking(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 15));

        var overlaps = existing.Overlaps(
            new DateTime(2026, 1, 1),
            new DateTime(2026, 1, 20));

        overlaps.Should().BeTrue();
    }

    [Fact]
    public void Should_Return_False_When_Booking_Touches_End_Date()
    {
        var existing = CreateBooking(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 15));

        var overlaps = existing.Overlaps(
            new DateTime(2026, 1, 15),
            new DateTime(2026, 1, 18));

        overlaps.Should().BeFalse();
    }

    [Fact]
    public void Should_Throw_Exception_When_StartDate_Is_After_EndDate()
    {
        // Act
        Action act = () => CreateBooking(
            new DateTime(2026, 1, 15),
            new DateTime(2026, 1, 10));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid date range");
    }

    [Fact]
    public void Should_Throw_Exception_When_StartDate_Equals_EndDate()
    {
        // Act
        Action act = () => CreateBooking(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 10));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid date range");
    }

    [Fact]
    public void Should_Calculate_TotalPrice_Correctly()
    {
        // Arrange
        var pricePerNight = 150m;

        // Act
        var booking = new Booking(
            _propertyId,
            _userId,
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 15),
            pricePerNight);

        // Assert
        booking.TotalPrice.Should().Be(750m); // 5 nights * 150
    }

    [Fact]
    public void Should_Set_Default_Status_To_Pending()
    {
        // Act
        var booking = CreateBooking(
            new DateTime(2026, 1, 10),
            new DateTime(2026, 1, 15));

        // Assert
        booking.BookingStatus.Should().Be(Status.Pending);
    }

    [Fact]
public void Should_Generate_Unique_BookingID()
{
    // Act
    var booking1 = CreateBooking(new DateTime(2026, 1, 10), new DateTime(2026, 1, 15));
    var booking2 = CreateBooking(new DateTime(2026, 2, 10), new DateTime(2026, 2, 15));

    // Assert
    booking1.BookingID.Should().NotBe(Guid.Empty);
    booking2.BookingID.Should().NotBe(Guid.Empty);
    booking1.BookingID.Should().NotBe(booking2.BookingID);
}
}
