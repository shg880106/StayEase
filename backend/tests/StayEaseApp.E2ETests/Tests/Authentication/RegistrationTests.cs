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
        await _registerPage.FillFullNameAsync(TestUsers.ValidUserToRegisterFullName);
        await _registerPage.FillEmailAsync(_generatedEmail);
        await _registerPage.FillPasswordAsync(TestUsers.ValidUserToRegisterPassword);
        await _registerPage.FillConfirmPasswordAsync(TestUsers.ValidUserToRegisterConfirmPassword);

        await _registerPage.CreateAsync();

        var userMenuButton = _registerPage.UserMenuButton(TestUsers.ValidUserToRegisterDisplayName);
        // Free-tier Azure SQL databases can be paused and need extra time to
        // resume (cold start) on the first request, so allow a longer timeout
        // than the Playwright default (5s) when running against remote/CI environments.
        await Expect(userMenuButton).ToBeVisibleAsync(new() { Timeout = 30000 });

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
        await _registerPage.FillFullNameAsync(TestUsers.ValidUserToRegisterFullName);
        await _registerPage.FillEmailAsync(TestUsers.ValidUserEmail);
        await _registerPage.FillPasswordAsync(TestUsers.ValidUserToRegisterPassword);
        await _registerPage.FillConfirmPasswordAsync(TestUsers.ValidUserToRegisterConfirmPassword);

        await _registerPage.CreateAsync();

        await Expect(_registerPage.RegistrationValidationError).ToBeVisibleAsync();        
    }

    [Test]
    [Category("Registration")]
    [Category("NegativePath")]
    public async Task RegisterUser_WithPasswordsNotMatch_ShowsValidationError()
    {
        await _registerPage.NavigateToRegisterAsync();
        await _registerPage.FillFullNameAsync(TestUsers.ValidUserToRegisterFullName);
        await _registerPage.FillEmailAsync(_generatedEmail);
        await _registerPage.FillPasswordAsync(TestUsers.ValidUserToRegisterPassword);
        await _registerPage.FillConfirmPasswordAsync(TestUsers.InValidUserToRegisterConfirmPassword);

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
