using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Configuration;
public static class TestEnvironment
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("TEST_BASEURL") ?? "http://localhost:4200/";

    public static string ApiUrl =>
        Environment.GetEnvironmentVariable("TEST_APIURL") ?? "http://localhost:7172/";
}
