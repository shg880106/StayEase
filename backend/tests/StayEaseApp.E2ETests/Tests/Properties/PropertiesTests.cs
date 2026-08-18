using Microsoft.Playwright;
using Microsoft.VisualBasic;
using StayEaseApp.E2ETests.Infrastructure;
using StayEaseApp.E2ETests.Pages;
using StayEaseApp.E2ETests.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Tests.Properties;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PropertiesTests : E2ETestBase
{
    private LoginPageObject _loginPage = null!;
    private PropertiesPageObject _propertiesPage = null!;
    private string propertyTitle = $"Test Property {Guid.NewGuid()}";

    [SetUp]
    public void SetUpPageObject()
    {
        _loginPage = new LoginPageObject(Page);
        _propertiesPage = new PropertiesPageObject(Page);
    }

    [Test]
    public async Task LoginWithUserWithoutProperties_ShowsNoPropertiesYet()
    {
        await _loginPage.LoginAndOpenUserMenuAsync(TestUsers.ValidUserWithoutPropertiesEmail, TestUsers.ValidUserWithoutPropertiesPassword, TestUsers.ValidUserWithoutPropertiesDisplayName);

        await _propertiesPage.NavigateToMyPropertiesAsync();

        // Azure free-tier App Service/SQL can cold-start, so allow more time
        // than the Playwright default (5s) when running against remote/CI environments.
        await Expect(_propertiesPage.NoPropertiesYetHeading).ToBeVisibleAsync(new() { Timeout = 45000 });
    }

    [Test]
    public async Task LoginWithUserWithProperties_ShowsProperties()
    {
        await _loginPage.LoginAndOpenUserMenuAsync(TestUsers.ValidUserEmail, TestUsers.ValidUserPassword, TestUsers.ValidUserDisplayName);

        await _propertiesPage.NavigateToMyPropertiesAsync();

        await Expect(_propertiesPage.ManagePropertiesText).ToBeVisibleAsync();
        await Expect(_propertiesPage.PropertyCards.First).ToBeVisibleAsync(new() { Timeout = 45000 });

        var propertiesCount = await _propertiesPage.PropertyCards.CountAsync();
        Assert.That(propertiesCount, Is.GreaterThan(0), "Expected the user to have at least one property listed.");
    }

    [Test]
    public async Task CreateProperty_WithValidData_AddsPropertyToList()
    {
        await _loginPage.LoginAndOpenUserMenuAsync(TestUsers.ValidUserEmail, TestUsers.ValidUserPassword, TestUsers.ValidUserDisplayName);

        await _propertiesPage.NavigateToMyPropertiesAsync();

        await _propertiesPage.CreatePropertyAsync(
            title: propertyTitle,
            description: "Test Description",
            location: "Test Location",
            pricePerNight: "120",
            maxGuests: "4");

        var createdPropertyCard = Page.GetByText(propertyTitle);
        await Expect(createdPropertyCard).ToBeVisibleAsync(new() { Timeout = 45000 });
    }

    [Test]
    public async Task UpdateProperty_WithValidData_UpdatesPropertyInList()
    {
        await _loginPage.LoginAndOpenUserMenuAsync(TestUsers.ValidUserEmail, TestUsers.ValidUserPassword, TestUsers.ValidUserDisplayName);

        await _propertiesPage.NavigateToMyPropertiesAsync();

        await _propertiesPage.CreatePropertyAsync(
            title: propertyTitle,
            description: "Test Description",
            location: "Test Location",
            pricePerNight: "120",
            maxGuests: "4");

        var createdPropertyCard = Page.GetByText(propertyTitle);
        await Expect(createdPropertyCard).ToBeVisibleAsync(new() { Timeout = 45000 });

        var updatedPropertyTitle = $"{propertyTitle} updated";

        await _propertiesPage.UpdatePropertyAsync(
            currentTitle: propertyTitle,
            newTitle: updatedPropertyTitle,
            newDescription: "Test Description updated",
            newLocation: "Test Location updated",
            newPricePerNight: "100",
            newMaxGuests: "3");

        var updatedPropertyCard = Page.GetByText(updatedPropertyTitle);
        await Expect(updatedPropertyCard).ToBeVisibleAsync(new() { Timeout = 45000 });
    }
    
    [Test]
    public async Task DeleteProperty_WithValidData_RemovesPropertyFromList()
    {
        await _loginPage.LoginAndOpenUserMenuAsync(TestUsers.ValidUserEmail, TestUsers.ValidUserPassword, TestUsers.ValidUserDisplayName);

        await _propertiesPage.NavigateToMyPropertiesAsync();

        await _propertiesPage.CreatePropertyAsync(
            title: propertyTitle,
            description: "Test Description",
            location: "Test Location",
            pricePerNight: "120",
            maxGuests: "4");

        var createdPropertyCard = _propertiesPage.PropertyCardHeading(propertyTitle);
        await Expect(createdPropertyCard).ToBeVisibleAsync(new() { Timeout = 45000 });

        var deletedPropertyTitle = $"{propertyTitle}";

        var deletedPropertyCard = _propertiesPage.PropertyCardHeading(deletedPropertyTitle);
        await Expect(deletedPropertyCard).ToBeVisibleAsync(new() { Timeout = 45000 });

        await _propertiesPage.DeletePropertyAsync(currentTitle: deletedPropertyTitle);
        await Expect(_propertiesPage.DeleteSuccessMessage(deletedPropertyTitle)).ToBeVisibleAsync(new() { Timeout = 45000 });
    }
}
