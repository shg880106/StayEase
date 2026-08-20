using StayEaseApp.E2ETests.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Infrastructure;
public class E2ETestBase : PageTest
{
    protected static readonly string BaseUrl = TestEnvironment.GetBaseUrl();
    protected static readonly string ApiUrl = TestEnvironment.GetApiUrl();

    private static readonly string ArtifactsDirectory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "playwright-artifacts");

    [SetUp]
    public async Task BaseSetUpAsync()
    {
        await Context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

        await Page.GotoAsync(BaseUrl);
        await Page.EvaluateAsync("() => localStorage.clear()");
    }

    [TearDown]
    public async Task BaseTearDownAsync()
    {
        var testFailed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed;

        Directory.CreateDirectory(ArtifactsDirectory);
        var safeTestName = string.Join("_", TestContext.CurrentContext.Test.Name.Split(Path.GetInvalidFileNameChars()));

        if (testFailed)
        {
            var tracePath = Path.Combine(ArtifactsDirectory, $"{safeTestName}-trace.zip");
            await Context.Tracing.StopAsync(new() { Path = tracePath });
            TestContext.AddTestAttachment(tracePath, "Playwright trace");

            var screenshotPath = Path.Combine(ArtifactsDirectory, $"{safeTestName}-screenshot.png");
            await Page.ScreenshotAsync(new() { Path = screenshotPath, FullPage = true });
            TestContext.AddTestAttachment(screenshotPath, "Failure screenshot");
        }
        else
        {
            // Stop tracing without persisting it to keep the artifacts folder clean on passing runs.
            await Context.Tracing.StopAsync();
        }
    }
}
