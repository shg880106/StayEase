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

    [SetUp]
    public async Task BaseSetUpAsync()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.EvaluateAsync("() => localStorage.clear()");
    }
}
