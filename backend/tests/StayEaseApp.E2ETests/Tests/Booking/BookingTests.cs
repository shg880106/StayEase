using Microsoft.Playwright;
using StayEaseApp.E2ETests.Infrastructure;
using StayEaseApp.E2ETests.Pages;
using StayEaseApp.E2ETests.TestData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Tests.Booking;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class BookingTests : E2ETestBase
{
    private LoginPageObject _loginPage = null!;
    private BookingPageObject _bookingPage = null!;
    private PropertiesPageObject _propertiesPage = null!;

    [SetUp]
    public void SetUpPageObject()
    {
        _loginPage = new LoginPageObject(Page);
        _bookingPage = new BookingPageObject(Page);
        _propertiesPage = new PropertiesPageObject(Page);
    }

    [Test]
    public async Task LoginWithUserWithoutBookings_ShowsNoBookingsYet()
    {
        await _loginPage.LoginAndOpenUserMenuAsync(TestUsers.ValidUserWithoutBookingsEmail, TestUsers.ValidUserWithoutBookingsPassword, TestUsers.ValidUserWithoutBookingsDisplayName);
        
        await _bookingPage.NavigateToMyBookingsAsync();
        await Expect(_bookingPage.NoBookingsYetHeading).ToBeVisibleAsync(new() { Timeout = 45000 });
    }

    [Test]
    public async Task LoginWithUserWithBookings_ShowsBookings()
    {
        await _loginPage.LoginAndOpenUserMenuAsync(TestUsers.ValidUserEmail, TestUsers.ValidUserPassword, TestUsers.ValidUserDisplayName);
        
        await _bookingPage.NavigateToMyBookingsAsync();
        await Expect(_bookingPage.ManageBookingsText).ToBeVisibleAsync();
        await Expect(_bookingPage.BookingCards.First).ToBeVisibleAsync(new() { Timeout = 45000 });

        var bookingsCount = await _bookingPage.BookingCards.CountAsync();
        Assert.That(bookingsCount, Is.GreaterThan(0), "Expected the user to have at least one booking listed.");
    }

    [Test]
    public async Task CreateBooking_WithValidData()
    {
        // Log in with ValidUserEmail
        await _loginPage.LoginAndOpenUserMenuAsync(TestUsers.ValidUserEmail, TestUsers.ValidUserPassword, TestUsers.ValidUserDisplayName);

        // Create property
        await _propertiesPage.NavigateToMyPropertiesAsync();
        string propertyTitle = $"Test Property {Guid.NewGuid()}";
        await _propertiesPage.CreatePropertyAsync(
            title: propertyTitle,
            description: "Test Description",
            location: "Test Location",
            pricePerNight: "120",
            maxGuests: "4");

        // Log out
        await _loginPage.OpenUserMenuAsync(TestUsers.ValidUserDisplayName);
        await _loginPage.LogOutAsync();
        await Expect(_loginPage.SignInNavLink).ToBeVisibleAsync(new() { Timeout = 45000 });

        // Log in with ValidUserWithBookingsEmail
        await _loginPage.LoginAsync(TestUsers.ValidUserWithBookingsEmail, TestUsers.ValidUserWithBookingsPassword);
        
        // Go to browse properties
        await _bookingPage.NavigateToBrowsePropertiesAsync();
        await _bookingPage.SelectPropertyAsync(propertyTitle);

        // Fill in check-in/check-out dates using dynamic dates relative to today,
        // so the test keeps working regardless of when it runs.
        var checkIn = DateTime.Today.AddDays(1);
        var checkOut = DateTime.Today.AddDays(8);
        await _bookingPage.FillBookingDatesAsync(
            checkIn: checkIn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            checkOut: checkOut.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

        // Verify the estimated total before confirming (7 nights x $120 = $840)
        await Expect(_bookingPage.EstimatedTotal).ToHaveTextAsync("$840");

        // Confirm the booking
        await _bookingPage.ConfirmBookingAsync();

        await Expect(_bookingPage.BookingConfirmedHeading).ToBeVisibleAsync();        
    }

    
}