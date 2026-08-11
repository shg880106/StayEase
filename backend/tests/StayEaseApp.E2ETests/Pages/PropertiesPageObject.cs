using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Pages;
public class PropertiesPageObject
{
    private readonly IPage _page;

    public PropertiesPageObject(IPage page)
    {
        _page = page;
    }

    private ILocator MyPropertiesLink => _page.GetByRole(AriaRole.Link, new() { NameString = "My Properties" });
    private ILocator NoPropertiesHeading => _page.GetByRole(AriaRole.Heading, new() { NameString = "No properties yet" });

    public ILocator NoPropertiesYetHeading => NoPropertiesHeading;

    public async Task NavigateToMyPropertiesAsync()
    {
        await MyPropertiesLink.ClickAsync();
        // Azure free-tier App Service/SQL can cold-start, so wait for the SPA route
        // to actually finish navigating before asserting on the resulting page state.
        await _page.WaitForURLAsync("**/my-properties", new() { Timeout = 30000 });
    }
}
