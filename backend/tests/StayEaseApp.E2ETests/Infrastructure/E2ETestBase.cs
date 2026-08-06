using StayEaseApp.E2ETests.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Infrastructure;
public class E2ETestBase : PageTest
{
    protected const string BaseUrl = TestEnvironment.BaseUrl;

    [SetUp]
    public async Task BaseSetUpAsync()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.EvaluateAsync("() => localStorage.clear()");
    }
}
