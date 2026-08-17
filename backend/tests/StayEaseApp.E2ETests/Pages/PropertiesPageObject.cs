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
    private ILocator AddPropertyButton => _page.GetByRole(AriaRole.Button, new() { NameString = "Add Property" });
    private ILocator TitleInput => _page.GetByRole(AriaRole.Textbox, new() { NameString = "Cozy mountain cabin" });
    private ILocator DescriptionInput => _page.GetByRole(AriaRole.Textbox, new() { NameString = "Describe your property…" });
    private ILocator LocationInput => _page.GetByRole(AriaRole.Textbox, new() { NameString = "Barcelona, Spain" });
    private ILocator PriceInput => _page.GetByPlaceholder("120");
    private ILocator MaxGuestsInput => _page.GetByPlaceholder("4");
    private ILocator CreatePropertyButton => _page.GetByRole(AriaRole.Button, new() { NameString = "Create Property" });
    private ILocator SaveChangesButton => _page.GetByRole(AriaRole.Button, new() { NameString = "Save Changes" });
    private ILocator DeleteConfirmButton => _page.GetByRole(AriaRole.Button, new() { NameString = "Yes, delete" });

    public ILocator NoPropertiesYetHeading => NoPropertiesHeading;
    public ILocator ManagePropertiesText => ManagePropertiesDescription;
    public ILocator PropertyCards => PropertiesGrid.Locator("> div");

    private ILocator UpdateButtonForProperty(string propertyTitle) =>
        PropertyCards.Filter(new() { HasTextString = propertyTitle }).GetByTitle("Update property");
    
    private ILocator DeleteButtonForProperty(string propertyTitle) =>
        PropertyCards.Filter(new() { HasTextString = propertyTitle }).GetByTitle("Delete property");

    public ILocator DeleteSuccessMessage(string propertyTitle) =>
        _page.GetByText($"\"{propertyTitle}\" was deleted successfully.", new() { Exact = false });

    public ILocator PropertyCardHeading(string propertyTitle) =>
        _page.GetByRole(AriaRole.Heading, new() { NameString = propertyTitle });

    public async Task NavigateToMyPropertiesAsync()
    {
        await MyPropertiesLink.ClickAsync();        
    }

    public async Task FillPropertyForm(string title, string description, string location, string pricePerNight, string maxGuests)
    {
        await TitleInput.ClickAsync();
        await TitleInput.FillAsync(title);
        await TitleInput.PressAsync("Tab");

        await DescriptionInput.FillAsync(description);
        await DescriptionInput.PressAsync("Tab");

        await LocationInput.FillAsync(location);
        await LocationInput.PressAsync("Tab");

        await PriceInput.FillAsync(pricePerNight);
        await PriceInput.PressAsync("Tab");

        await MaxGuestsInput.FillAsync(maxGuests);
    }

    public async Task CreatePropertyAsync(string title, string description, string location, string pricePerNight, string maxGuests)
    {
        await AddPropertyButton.ClickAsync();

        await FillPropertyForm(title, description, location, pricePerNight, maxGuests);

        await CreatePropertyButton.ClickAsync();
    }

    public async Task UpdatePropertyAsync(string currentTitle, string newTitle, string newDescription, string newLocation, string newPricePerNight, string newMaxGuests)
    {
        await UpdateButtonForProperty(currentTitle).ClickAsync();

        await FillPropertyForm(newTitle, newDescription, newLocation, newPricePerNight, newMaxGuests);
        
        await SaveChangesButton.ClickAsync();
    }
    
    public async Task DeletePropertyAsync(string currentTitle)
    {
        await DeleteButtonForProperty(currentTitle).ClickAsync();
        await DeleteConfirmButton.ClickAsync();        
    }
}
