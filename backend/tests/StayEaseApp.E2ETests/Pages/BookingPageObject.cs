using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Pages;
public class BookingPageObject
{
    private readonly IPage _page;

    public BookingPageObject(IPage page)
    {
        _page = page;
    }

    private ILocator BrowsePropertiesLink => _page.GetByRole(AriaRole.Link, new() { NameString = "Browse Properties" });
    private ILocator MyBookingsLink => _page.GetByRole(AriaRole.Link, new() { NameString = "My Bookings" });
    private ILocator NoBookingsHeading => _page.GetByRole(AriaRole.Heading, new() { NameString = "No bookings yet" });
    private ILocator ManageBookingsDescription => _page.GetByText("All reservations you have made on StayEase.");
    private ILocator SelectedProperty(string propertyTitle) => _page.GetByRole(AriaRole.Heading, new() { NameString = propertyTitle });
    // Every booking card renders a "Booking #<id>" text, regardless of which status group it belongs to.
    private ILocator BookingCardsLocator => _page.GetByText("Booking #", new() { Exact = false });
    private ILocator BookingConfirmedSuccess => _page.GetByRole(AriaRole.Heading, new() { NameString = "Booking Confirmed!" });
    private ILocator EstimatedTotalAmount => _page.Locator("span.text-rose-500", new() { HasTextString = "$" }).Last;
    private ILocator ConfirmBookingButton => _page.GetByRole(AriaRole.Button, new() { NameString = "Confirm Booking" });
    private ILocator CheckIn => _page.GetByLabel("Check-in");   
    private ILocator CheckOut => _page.GetByLabel("Check-out");


    public ILocator NoBookingsYetHeading => NoBookingsHeading;
    public ILocator ManageBookingsText => ManageBookingsDescription;
    public ILocator BookingCards => BookingCardsLocator;    
    public ILocator BookingConfirmedHeading => BookingConfirmedSuccess;
    public ILocator EstimatedTotal => EstimatedTotalAmount;

    public ILocator StatusGroupHeading(string statusLabel) =>
        _page.GetByRole(AriaRole.Heading, new() { NameString = statusLabel });

    public async Task NavigateToBrowsePropertiesAsync()
    {
        await BrowsePropertiesLink.ClickAsync();
    }

    public async Task NavigateToMyBookingsAsync()
    {
        await MyBookingsLink.ClickAsync();
    }

    public async Task SelectPropertyAsync(string propertyTitle)
    {
        //var propertyCard = _page.GetByRole(AriaRole.Heading, new() { NameString = propertyTitle });
        await SelectedProperty(propertyTitle).ClickAsync();
    }

    public async Task FillBookingDatesAsync(string checkIn, string checkOut)
    {
        await CheckIn.FillAsync(checkIn);
        await CheckOut.FillAsync(checkOut);
    }

    public async Task ConfirmBookingAsync()
    {
        await ConfirmBookingButton.ClickAsync();
    }

    public async Task CreateBookingAsync(string checkIn, string checkOut)
    {
        await FillBookingDatesAsync(checkIn, checkOut);
        await ConfirmBookingAsync();
    }
}
