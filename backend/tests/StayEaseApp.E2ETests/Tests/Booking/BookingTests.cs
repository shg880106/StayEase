using StayEaseApp.E2ETests.Infrastructure;
using StayEaseApp.E2ETests.Pages;
using StayEaseApp.E2ETests.TestData;
using System;
using System.Collections.Generic;
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

    [SetUp]
    public void SetUpPageObject()
    {
        _loginPage = new LoginPageObject(Page);
        _bookingPage = new BookingPageObject(Page);
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
}
