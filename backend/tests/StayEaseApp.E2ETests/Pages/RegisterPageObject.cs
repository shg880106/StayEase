using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Pages;
public class RegisterPageObject
{
    private readonly IPage _page;

    public RegisterPageObject(IPage page)
    {
        _page = page;
    }

    private ILocator RegisterInLink => _page.GetByRole(AriaRole.Link, new() { NameString = "Register" });
    private ILocator FullNameInLink => _page.GetByRole(AriaRole.Textbox, new () { NameString = "Jane Doe" });
    private ILocator EmailInput => _page.GetByRole(AriaRole.Textbox, new() { NameString = "you@example.com" });
    private ILocator PasswordInput => _page.GetByRole(AriaRole.Textbox, new() { NameString = "Min. 6 characters" });
    private ILocator ConfirmPasswordInput => _page.GetByRole(AriaRole.Textbox, new() { NameString = "Re-enter your password" });
    private ILocator CreateButton => _page.GetByRole(AriaRole.Button, new() { NameString = "Create Account" });
    private ILocator ValidationError => _page.GetByText("Email already registered");
    private ILocator PasswordsDoNotMatchError => _page.GetByText("Passwords do not match.");

    public ILocator FullNameField => FullNameInLink;
    public ILocator EmailField => EmailInput;
    public ILocator PasswordField => PasswordInput;
    public ILocator ConfirmPasswordField => ConfirmPasswordInput;
    public ILocator CreateLoginButton => CreateButton;
    public ILocator RegistrationValidationError => ValidationError;
    public ILocator PasswordsDoNotMatchValidationError => PasswordsDoNotMatchError;

    public async Task NavigateToRegisterAsync()
    {
        await RegisterInLink.ClickAsync();
    }

    public async Task FillFullNameAsync(string fullName)
    {
        await FullNameInLink.ClickAsync();
        await FullNameInLink.FillAsync(fullName);
        await FullNameInLink.PressAsync("Tab");
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

    public async Task FillConfirmPasswordAsync(string confirmPassword)
    {
        await ConfirmPasswordInput.FillAsync(confirmPassword);
    }

    public async Task CreateAsync()
    {
        await CreateButton.ClickAsync();
    }

    public ILocator UserMenuButton(string displayName) =>
        _page.GetByRole(AriaRole.Button, new() { NameString = displayName });

    public string CurrentUrl => _page.Url;
}
