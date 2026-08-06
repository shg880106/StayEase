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
public class RegistrationTests : E2ETestBase
{
    private RegisterPageObject _registerPage = null!;
    private string _generatedEmail = null!;

    [SetUp]
    public void SetUpPageObject()
    {
        _registerPage = new RegisterPageObject(Page);
        _generatedEmail = $"e2e-registration+{DateTime.UtcNow:yyyyMMddHHmmss}" +
                          $"-{Guid.NewGuid():N}@gmail.com";
    }

    [Test]
    [Category("Registration")]
    public async Task RegisterNewUser_WithValidData_ShowsSuccessUserName()
    {
        await _registerPage.NavigateToRegisterAsync();
        await _registerPage.FillFullNameAsync(TestEnvironment.ValidUserToRegisterFullName);
        await _registerPage.FillEmailAsync(_generatedEmail);
        await _registerPage.FillPasswordAsync(TestEnvironment.ValidUserToRegisterPassword);
        await _registerPage.FillConfirmPasswordAsync(TestEnvironment.ValidUserToRegisterConfirmPassword);

        await _registerPage.CreateAsync();

        var userMenuButton = _registerPage.UserMenuButton(TestEnvironment.ValidUserToRegisterDisplayName);
        await Expect(userMenuButton).ToBeVisibleAsync();

        // Negative-space checks: ensure the registration form/page has actually
        // been navigated away from, not just hidden behind the user menu.
        await Expect(_registerPage.CreateLoginButton).Not.ToBeVisibleAsync();
        Assert.That(_registerPage.CurrentUrl, Does.Not.Contain("/register"));
    }    

    [Test]
    [Category("Registration")]
    [Category("NegativePath")]
    public async Task RegisterUser_WithExistingEmail_ShowsValidationError()
    {
        await _registerPage.NavigateToRegisterAsync();
        await _registerPage.FillFullNameAsync(TestEnvironment.ValidUserToRegisterFullName);
        await _registerPage.FillEmailAsync(TestEnvironment.ValidUserEmail);
        await _registerPage.FillPasswordAsync(TestEnvironment.ValidUserToRegisterPassword);
        await _registerPage.FillConfirmPasswordAsync(TestEnvironment.ValidUserToRegisterConfirmPassword);

        await _registerPage.CreateAsync();

        await Expect(_registerPage.RegistrationValidationError).ToBeVisibleAsync();        
    }

    [Test]
    [Category("Registration")]
    [Category("NegativePath")]
    public async Task RegisterUser_WithPasswordsNotMatch_ShowsValidationError()
    {
        await _registerPage.NavigateToRegisterAsync();
        await _registerPage.FillFullNameAsync(TestEnvironment.ValidUserToRegisterFullName);
        await _registerPage.FillEmailAsync(_generatedEmail);
        await _registerPage.FillPasswordAsync(TestEnvironment.ValidUserToRegisterPassword);
        await _registerPage.FillConfirmPasswordAsync(TestEnvironment.InValidUserToRegisterConfirmPassword);

        await _registerPage.CreateAsync();

        await Expect(_registerPage.PasswordsDoNotMatchValidationError).ToBeVisibleAsync();
    }

    [TearDown]
    public void ReportCreatedTestUser()
    {
        TestContext.Progress.WriteLine(
            $"E2E registration test finished. Generated email: {_generatedEmail}");
    }
}
