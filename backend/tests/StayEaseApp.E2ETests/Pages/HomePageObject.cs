using Microsoft.Playwright;
using StayEaseApp.E2ETests.Configuration;
using StayEaseApp.E2ETests.Infrastructure;

namespace StayEaseApp.E2ETests.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Smoke")]
public class HomePageObject
{
    private readonly IPage _page;

    public HomePageObject(IPage page)
    {
        _page = page;
    }

    public ILocator HomeComponent => _page.Locator("app-home");
    public ILocator HeroHeading => _page.Locator("h1");
    public ILocator SignInLink => _page.GetByRole(AriaRole.Link, new() { NameString = "Sign In" });
    public ILocator RegisterLink => _page.GetByRole(AriaRole.Link, new() { NameString = "Register" });
}
