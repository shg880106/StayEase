using Microsoft.Playwright;
using StayEaseApp.E2ETests.Configuration;

namespace StayEaseApp.E2ETests.Pages;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    private const string BaseUrl = TestEnvironment.BaseUrl;

    [SetUp]
    public async Task SetUp()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.EvaluateAsync("() => localStorage.clear()");
    }

    [Test]
    public async Task HomePageCanBeOpened()
    {
        await Page.GotoAsync(BaseUrl);

        var response = await Page.GotoAsync(BaseUrl);

        Assert.That(
            response,
            Is.Not.Null,
            "The Angular application did not return an HTTP response.");

        Assert.That(
            response!.Ok,
            Is.True,
            $"The homepage returned HTTP status {response.Status}.");

        await Expect(Page).ToHaveURLAsync(BaseUrl);

        var homeComponent = Page.Locator("app-home");
        await Expect(homeComponent).ToBeVisibleAsync();
        await Expect(homeComponent).ToContainTextAsync("Property Booking Platform");

        await Expect(Page.Locator("h1")).ToContainTextAsync("Find your perfect stay, effortlessly.");

        var signInLink = Page.GetByRole(AriaRole.Link, new() { NameString = "Sign In" });
        await Expect(signInLink).ToBeVisibleAsync();
        await Expect(signInLink).ToBeEnabledAsync();

        var registerLink = Page.GetByRole(AriaRole.Link, new() { NameString = "Register" });
        await Expect(registerLink).ToBeVisibleAsync();
        await Expect(registerLink).ToBeEnabledAsync();
    }

    
}
