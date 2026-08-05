using Microsoft.Playwright;

namespace StayEaseApp.E2ETests.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    [Test]
    public async Task HomePageShowsCorrectly()
    {
        await Page.GotoAsync("http://localhost:4200/");
        await Expect(Page.GetByRole(AriaRole.Link, new() { NameString = "StayEase" })).ToBeVisibleAsync();
        await Expect(Page.Locator("app-home")).ToContainTextAsync("Property Booking Platform");
        await Expect(Page.Locator("h1")).ToContainTextAsync("Find your perfect stay, effortlessly.");
        await Expect(Page.GetByRole(AriaRole.Link, new() { NameString = "Sign In" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { NameString = "Register" })).ToBeVisibleAsync();
    }

    
}
