using Microsoft.Playwright;
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

    [SetUp]
    public void SetUpPageObject()
    {
        _loginPage = new LoginPageObject(Page);
        _propertiesPage = new PropertiesPageObject(Page);
    }

    [Test]
    public async Task LoginWithUserWithoutProperties_ShowsNoPropertiesYet()
    {
        await _loginPage.LoginAsync(TestUsers.ValidUserWithoutPropertiesEmail, TestUsers.ValidUserWithoutPropertiesPassword);

        var userMenuButton = _loginPage.UserMenuButton(TestUsers.ValidUserWithoutPropertiesDisplayName);
        // Azure free-tier App Service/SQL can cold-start, so allow more time
        // than the Playwright default (5s) when running against remote/CI environments.
        await Expect(userMenuButton).ToBeVisibleAsync(new() { Timeout = 30000 });

        await _loginPage.OpenUserMenuAsync(TestUsers.ValidUserWithoutPropertiesDisplayName);
        await _propertiesPage.NavigateToMyPropertiesAsync();

        // Azure free-tier App Service/SQL can cold-start, so allow more time
        // than the Playwright default (5s) when running against remote/CI environments.
        await Expect(_propertiesPage.NoPropertiesYetHeading).ToBeVisibleAsync(new() { Timeout = 45000 });
    }

    [Test]
    public async Task LoginWithUserWithProperties_ShowsProperties()
    {
        await _loginPage.LoginAsync(TestUsers.ValidUserEmail, TestUsers.ValidUserPassword);

        var userMenuButton = _loginPage.UserMenuButton(TestUsers.ValidUserDisplayName);
        // Azure free-tier App Service/SQL can cold-start, so allow more time
        // than the Playwright default (5s) when running against remote/CI environments.
        await Expect(userMenuButton).ToBeVisibleAsync(new() { Timeout = 30000 });

        await _loginPage.OpenUserMenuAsync(TestUsers.ValidUserDisplayName);
        await _propertiesPage.NavigateToMyPropertiesAsync();

        await Expect(_propertiesPage.ManagePropertiesText).ToBeVisibleAsync();
        await Expect(_propertiesPage.PropertyCards.First).ToBeVisibleAsync(new() { Timeout = 45000 });

        var propertiesCount = await _propertiesPage.PropertyCards.CountAsync();
        Assert.That(propertiesCount, Is.GreaterThan(0), "Expected the user to have at least one property listed.");

    }
}
