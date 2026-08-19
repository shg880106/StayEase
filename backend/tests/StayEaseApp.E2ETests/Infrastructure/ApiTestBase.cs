using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using StayEaseApp.Application.DTOs;
using StayEaseApp.E2ETests.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Infrastructure;
public class ApiTestBase : PlaywrightTest
{
    protected IAPIRequestContext Request { get; private set; } = null!;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [SetUp]
    public async Task SetUpAsync()
    {
        var apiUrl = TestEnvironment.GetApiUrl();
        var isLocal = apiUrl.Contains("localhost");

        Request = await Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = apiUrl,
            IgnoreHTTPSErrors = isLocal
        });
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await Request.DisposeAsync();
    }

    protected static async Task<T?> ReadAsAsync<T>(IAPIResponse response)
    {
        var body = await response.TextAsync();
        return JsonSerializer.Deserialize<T>(body, JsonOptions);
    }

    protected async Task<string> GetAuthTokenAsync(string email, string password)
    {
        var response = await Request.PostAsync("api/Auth/login", new APIRequestContextOptions
        {
            DataObject = new { email, password }
        });

        if (!response.Ok)
        {
            throw new Exception($"Login failed with status {response.Status}: {await response.TextAsync()}");
        }

        var authResponse = await ReadAsAsync<AuthResponseDto>(response);
        return authResponse!.Token;
    }

}
