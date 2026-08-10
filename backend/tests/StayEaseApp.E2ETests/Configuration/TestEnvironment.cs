using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Configuration;
public static class TestEnvironment
{
    public static string GetBaseUrl()
    {
        // CI: Read from environment variable
        var envUrl = Environment.GetEnvironmentVariable("TEST_BASEURL");
        if (!string.IsNullOrEmpty(envUrl))
        {
            return envUrl;
        }

        // Local dev: Try .runsettings parameters
        var paramUrl = TestContext.Parameters.Get("BaseUrl", null);
        if (!string.IsNullOrEmpty(paramUrl))
        {
            return paramUrl;
        }

        // Final fallback for local dev
        return "http://localhost:4200/";
    }
    public static string GetApiUrl()
    {
        // CI: Read from environment variable
        var envUrl = Environment.GetEnvironmentVariable("TEST_APIURL");
        if (!string.IsNullOrEmpty(envUrl))
        {
            return envUrl;
        }

        // Local dev: Try .runsettings parameters
        var paramUrl = TestContext.Parameters.Get("ApiUrl", null);
        if (!string.IsNullOrEmpty(paramUrl))
        {
            return paramUrl;
        }

        // Final fallback for local dev
        return "http://localhost:7172";
    }
}
