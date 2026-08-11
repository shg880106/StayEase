using Microsoft.Playwright;
using StayEaseApp.E2ETests.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Pages;

public class LoginPageObject
{
    private readonly IPage _page;

    public LoginPageObject(IPage page)
    {
        _page = page;
    }

    private ILocator RegisterLink => _page.GetByRole(AriaRole.Link, new() { NameString = "Register" });
    private ILocator SignInLink => _page.GetByRole(AriaRole.Link, new() { NameString = "Sign In" });
    private ILocator EmailInput => _page.GetByRole(AriaRole.Textbox, new() { NameString = "you@example.com" });
    private ILocator PasswordInput => _page.GetByRole(AriaRole.Textbox, new() { NameString = "••••••••" });
    private ILocator ShowPasswordButton => _page.GetByRole(AriaRole.Button, new() { NameString = "Show password" });
    private ILocator HidePasswordButton => _page.GetByRole(AriaRole.Button, new() { NameString = "Hide password" });
    private ILocator SubmitButton => _page.GetByRole(AriaRole.Button, new() { NameString = "Sign In" });
    private ILocator ValidationError => _page.GetByText("Invalid email or password");
    private ILocator UserMenuTrigger => _page.Locator("#user-menu-trigger");
    private ILocator EmailRequiredError => _page.GetByText("Email is required.");
    private ILocator PasswordRequiredError => _page.GetByText("Password is required.");
    private ILocator LogOutButton => _page.GetByRole(AriaRole.Button, new() { NameString = "Sign Out" });

    public ILocator EmailField => EmailInput;
    public ILocator PasswordField => PasswordInput;
    public ILocator SubmitLoginButton => SubmitButton;
    public ILocator LoginValidationError => ValidationError;
    public ILocator UserMenu => UserMenuTrigger;
    public ILocator EmailRequiredValidation => EmailRequiredError;
    public ILocator PasswordRequiredValidation => PasswordRequiredError;
    public ILocator SignInNavLink => SignInLink;
    public ILocator RegisterNavLink => RegisterLink;


    public async Task NavigateToLoginAsync()
    {
        await SignInLink.ClickAsync();
    }

    public async Task FillEmailAsync(string email)
    {
        await EmailInput.ClickAsync();
        await EmailInput.FillAsync(email);
        await EmailInput.PressAsync("Tab");
    }

    public async Task FillPasswordAsync(string password)
    {
        await PasswordInput.FillAsync(password);
    }

    public async Task ToggleShowPasswordAsync()
    {
        await ShowPasswordButton.ClickAsync();
    }

    public async Task ToggleHidePasswordAsync()
    {
        await HidePasswordButton.ClickAsync();
    }

    public async Task SubmitAsync()
    {
        await SubmitButton.ClickAsync();
    }

    public async Task LoginAsync(string email, string password)
    {
        await NavigateToLoginAsync();
        await FillEmailAsync(email);
        await FillPasswordAsync(password);
        await SubmitAsync();
    }

    public ILocator UserMenuButton(string displayName) =>
        _page.GetByRole(AriaRole.Button, new() { NameString = displayName });

    public async Task OpenUserMenuAsync(string displayName)
    {
        await UserMenuButton(displayName).ClickAsync();
    }

    public async Task LogOutAsync()
    {
        await LogOutButton.ClickAsync();
    }    
}
