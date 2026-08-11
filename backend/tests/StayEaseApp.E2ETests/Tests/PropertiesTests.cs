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
