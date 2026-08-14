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

    private ILocator MyBookingsLink => _page.GetByRole(AriaRole.Link, new() { NameString = "My Bookings" });
    private ILocator NoBookingsHeading => _page.GetByRole(AriaRole.Heading, new() { NameString = "No bookings yet" });
    
    // Every booking card renders a "Booking #<id>" text, regardless of which status group it belongs to.
    private ILocator BookingCardsLocator => _page.GetByText("Booking #", new() { Exact = false });

    private ILocator ManageBookingsDescription => _page.GetByText("All reservations you have made on StayEase.");
   
    public ILocator NoBookingsYetHeading => NoBookingsHeading;
    public ILocator ManageBookingsText => ManageBookingsDescription;
    public ILocator BookingCards => BookingCardsLocator;

    public ILocator StatusGroupHeading(string statusLabel) =>
        _page.GetByRole(AriaRole.Heading, new() { NameString = statusLabel });

    public async Task NavigateToMyBookingsAsync()
    {
        await MyBookingsLink.ClickAsync();
    }
}
