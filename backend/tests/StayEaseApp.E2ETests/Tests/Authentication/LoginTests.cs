using Microsoft.Playwright;
using StayEaseApp.E2ETests.Configuration;
using StayEaseApp.E2ETests.Infrastructure;
using StayEaseApp.E2ETests.Pages;
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
        await _loginPage.FillEmailAsync(TestEnvironment.ValidUserEmail);
        await _loginPage.FillPasswordAsync(TestEnvironment.ValidUserPassword);

        await _loginPage.ToggleShowPasswordAsync();
        await Expect(_loginPage.PasswordField).ToHaveAttributeAsync("type", "text");

        await _loginPage.ToggleHidePasswordAsync();
        await Expect(_loginPage.PasswordField).ToHaveAttributeAsync("type", "password");

        await _loginPage.SubmitAsync();

        var userMenuButton = _loginPage.UserMenuButton(TestEnvironment.ValidUserDisplayName);
        await Expect(userMenuButton).ToBeVisibleAsync();

        await _loginPage.OpenUserMenuAsync(TestEnvironment.ValidUserDisplayName);

        await Expect(_loginPage.UserMenu).ToContainTextAsync(TestEnvironment.ValidUserDisplayName);
    }

    [Test]
    [Category("Authentication")]
    [Category("NegativePath")]
    public async Task LoginInToTheApplication_WithInvalidCredentials_ShowsValidationError()
    {
        await _loginPage.NavigateToLoginAsync();
        await _loginPage.FillEmailAsync(TestEnvironment.InvalidUserEmail);
        await _loginPage.FillPasswordAsync(TestEnvironment.InvalidUserPassword);

        await _loginPage.SubmitAsync();

        await Expect(_loginPage.LoginValidationError).ToBeVisibleAsync();
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
