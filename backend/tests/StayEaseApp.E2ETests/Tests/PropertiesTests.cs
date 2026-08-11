using Microsoft.Playwright;
using StayEaseApp.E2ETests.Infrastructure;
using StayEaseApp.E2ETests.Pages;
using StayEaseApp.E2ETests.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Smoke")]
public class PropertiesTests : E2ETestBase
{
    private RegisterPageObject _registerPage = null!;
    private PropertiesPageObject _propertiesPage = null!;

    [SetUp]
    public void SetUpPageObject()
    {
        _registerPage = new RegisterPageObject(Page);
        _propertiesPage = new PropertiesPageObject(Page);
    }

    [Test]
    public async Task LoginWithUserWithoutProperties_ShowsNoPropertiesYet()
    {
        // Register a brand-new user for this test run to guarantee the account
        // has no properties, rather than relying on a shared fixture user whose
        // state can drift over time as other tests/manual runs create properties.
        var displayName = $"E2E NoProps {Guid.NewGuid():N}"[..20];
        var email = $"e2e-empty-properties+{Guid.NewGuid():N}@stayease.test";
        const string password = "asd1234";

        await _registerPage.NavigateToRegisterAsync();
        await _registerPage.FillFullNameAsync(displayName);
        await _registerPage.FillEmailAsync(email);
        await _registerPage.FillPasswordAsync(password);
        await _registerPage.FillConfirmPasswordAsync(password);
        await _registerPage.CreateAsync();

        var userMenuButton = _registerPage.UserMenuButton(displayName);
        // Azure free-tier App Service/SQL can cold-start, so allow more time
        // than the Playwright default (5s) when running against remote/CI environments.
        await Expect(userMenuButton).ToBeVisibleAsync(new() { Timeout = 30000 });

        await userMenuButton.ClickAsync();
        await _propertiesPage.NavigateToMyPropertiesAsync();

        await Expect(_propertiesPage.NoPropertiesYetHeading).ToBeVisibleAsync(new() { Timeout = 30000 });
    }

    //[Test]
    //public async Task MyTest()
    //{
    //    await Page.GotoAsync("http://localhost:4200/");
    //    await Page.GetByRole(AriaRole.Link, new() { NameString = "Sign In" }).ClickAsync();
    //    await Page.GetByRole(AriaRole.Textbox, new() { NameString = "you@example.com" }).ClickAsync();
    //    await Page.GetByRole(AriaRole.Textbox, new() { NameString = "you@example.com" }).FillAsync("e2e-empty-properties@stayease.test");
    //    await Page.GetByRole(AriaRole.Textbox, new() { NameString = "••••••••" }).FillAsync("asd1234");
    //    await Page.GetByRole(AriaRole.Button, new() { NameString = "Sign In" }).ClickAsync();
    //    await Page.GetByRole(AriaRole.Button, new() { NameString = "E Empty Properties" }).ClickAsync();
    //    await Page.GetByRole(AriaRole.Link, new() { NameString = "My Properties" }).ClickAsync();
    //    await Expect(Page.GetByRole(AriaRole.Heading, new() { NameString = "No properties yet" })).ToBeVisibleAsync();
    //}
}
