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
    private ILocator ManagePropertiesDescription => _page.GetByText("Manage the properties you own");
    private ILocator PropertiesGrid => _page.Locator("div.grid.sm\\:grid-cols-2.lg\\:grid-cols-3.gap-6");

    public ILocator NoPropertiesYetHeading => NoPropertiesHeading;
    public ILocator ManagePropertiesText => ManagePropertiesDescription;
    public ILocator PropertyCards => PropertiesGrid.Locator("> div");

    public async Task NavigateToMyPropertiesAsync()
    {
        await MyPropertiesLink.ClickAsync();        
    }
}
