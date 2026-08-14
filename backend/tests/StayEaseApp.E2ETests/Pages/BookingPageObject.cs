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

    public ILocator NoBookingsYetHeading => NoBookingsHeading;

    public async Task NavigateToMyBookingsAsync()
    {
        await MyBookingsLink.ClickAsync();
    }
}
