using Microsoft.Playwright;
using StayEaseApp.E2ETests.Configuration;
using StayEaseApp.E2ETests.Infrastructure;
using StayEaseApp.E2ETests.Pages;
using StayEaseApp.E2ETests.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Tests.Authentication;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Smoke")]
public class LoginTests : E2ETestBase
{
    private LoginPageObject _loginPage = null!;

    [SetUp]
    public void SetUpPageObject()
    {
        _loginPage = new LoginPageObject(Page);
    }

    [Test]
    [Category("Authentication")]
    public async Task LoginInToTheApplication_WithValidCredentials_ShowsUserMenu()
    {
        await _loginPage.NavigateToLoginAsync();
        await _loginPage.FillEmailAsync(TestUsers.ValidUserEmail);
        await _loginPage.FillPasswordAsync(TestUsers.ValidUserPassword);

        await _loginPage.ToggleShowPasswordAsync();
        await Expect(_loginPage.PasswordField).ToHaveAttributeAsync("type", "text");

        await _loginPage.ToggleHidePasswordAsync();
        await Expect(_loginPage.PasswordField).ToHaveAttributeAsync("type", "password");

        await _loginPage.SubmitAsync();

        var userMenuButton = _loginPage.UserMenuButton(TestUsers.ValidUserDisplayName);
        // Azure free-tier App Service/SQL can cold-start, so allow more time
        // than the Playwright default (5s) when running against remote/CI environments.
        await Expect(userMenuButton).ToBeVisibleAsync(new() { Timeout = 30000 });

        await _loginPage.OpenUserMenuAsync(TestUsers.ValidUserDisplayName);

        await Expect(_loginPage.UserMenu).ToContainTextAsync(TestUsers.ValidUserDisplayName);
    }

    [Test]
    [Category("Authentication")]
    [Category("NegativePath")]
    public async Task LoginInToTheApplication_WithInvalidCredentials_ShowsValidationError()
    {
        await _loginPage.NavigateToLoginAsync();
        await _loginPage.FillEmailAsync(TestUsers.InvalidUserEmail);
        await _loginPage.FillPasswordAsync(TestUsers.InvalidUserPassword);

        await _loginPage.SubmitAsync();

        // Azure free-tier App Service/SQL can cold-start, so allow more time
        // than the Playwright default (5s) when running against remote/CI environments.
        await Expect(_loginPage.LoginValidationError).ToBeVisibleAsync(new() { Timeout = 30000 });
    }

    [Test]
    [Category("Authentication")]
    [Category("NegativePath")]
    public async Task LoginInToTheApplication_WithEmptyFields_KeepsSubmitDisabledOrShowsValidation()
    {
        await _loginPage.NavigateToLoginAsync();

        await _loginPage.SubmitAsync();
                
        await Expect(_loginPage.EmailRequiredValidation).ToBeVisibleAsync();
        await Expect(_loginPage.PasswordRequiredValidation).ToBeVisibleAsync();        
    }
}
