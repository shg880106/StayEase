using StayEaseApp.E2ETests.Configuration;
using StayEaseApp.E2ETests.Infrastructure;
using StayEaseApp.E2ETests.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Tests.Home;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Smoke")]
public class HomeTests : E2ETestBase
{
    private HomePageObject _homePage = null!;

    [SetUp]
    public void SetUpPageObject()
    {
        _homePage = new HomePageObject(Page);
    }

    [Test]
    public async Task HomePageCanBeOpened()
    {
        var response = await Page.GotoAsync(TestEnvironment.GetBaseUrl());

        Assert.That(
            response,
            Is.Not.Null,
            "The Angular application did not return an HTTP response.");

        Assert.That(
            response!.Ok,
            Is.True,
            $"The homepage returned HTTP status {response.Status}.");

        await Expect(Page).ToHaveURLAsync(TestEnvironment.GetBaseUrl());
        await Expect(_homePage.HomeComponent).ToBeVisibleAsync();
        await Expect(_homePage.HomeComponent).ToContainTextAsync("Property Booking Platform");
        await Expect(_homePage.HeroHeading).ToContainTextAsync("Find your perfect stay, effortlessly.");
    }

    [Test]
    public async Task HomePage_ShowsSignInAndRegisterLinks()
    {
        await Page.GotoAsync(TestEnvironment.GetBaseUrl());

        await Expect(_homePage.SignInLink).ToBeVisibleAsync();
        await Expect(_homePage.SignInLink).ToBeEnabledAsync();

        await Expect(_homePage.RegisterLink).ToBeVisibleAsync();
        await Expect(_homePage.RegisterLink).ToBeEnabledAsync();
    }
}
