using StayEaseApp.Application.DTOs;
using StayEaseApp.E2ETests.Configuration;
using StayEaseApp.E2ETests.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Tests.API;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Api")]
public class AuthApiTests
{
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        var apiUrl = TestEnvironment.GetApiUrl();
        var isLocal = apiUrl.Contains("localhost");

        var handler = new HttpClientHandler();
        if (isLocal)
        {
            // For local/dev environments only with a Kestrel self-signed certificate
            // Do not use this bypass against production or real CI environments
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        }

        _client = new HttpClient(handler)
        {
            BaseAddress = new Uri(apiUrl),
            DefaultRequestVersion = HttpVersion.Version11,
            DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact
        };
    }

    [TearDown]
    public void TearDown() => _client.Dispose();

    [Test]
    [Category("Registration")]
    public async Task Register_WithValidData_Returns200()
    {
        var generatedEmail = $"e2e-registration+{DateTime.UtcNow:yyyyMMddHHmmss}" +
                          $"-{Guid.NewGuid():N}@gmail.com";
        var request = new
        {
            name = TestUsers.ValidUserToRegisterFullName,
            email = generatedEmail,
            password = TestUsers.ValidUserToRegisterPassword
        };

        var response = await _client.PostAsJsonAsync("/api/Auth/register", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Registration")]
    [Category("NegativePath")]
    public async Task Register_WithExistingEmail_Returns400()
    {
        var request = new
        {
            name = TestUsers.ValidUserDisplayName,
            email = TestUsers.ValidUserEmail,
            password = TestUsers.ValidUserPassword
        };

        var response = await _client.PostAsJsonAsync("/api/Auth/register", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    [Category("Authentication")]
    public async Task LoginIn_WithValidCredentials_Returns200()
    {
        var request = new
        {
            email = TestUsers.ValidUserEmail,
            password = TestUsers.ValidUserPassword
        };

        var response = await _client.PostAsJsonAsync("/api/Auth/login", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    [Category("Authentication")]
    [Category("NegativePath")]
    public async Task LoginIn_WithInvalidCredentials_Returns401()
    {
        var request = new
        {
            email = TestUsers.InvalidUserEmail,
            password = TestUsers.InvalidUserPassword
        };

        var response = await _client.PostAsJsonAsync("/api/Auth/login", request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    [Category("Authentication")]
    public async Task LoginIn_WithValidCredentials_ReturnsTokenAndUserInfo()
    {
        var request = new
        {
            email = TestUsers.ValidUserEmail,
            password = TestUsers.ValidUserPassword
        };

        var response = await _client.PostAsJsonAsync("/api/Auth/login", request);
        var content = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content?.Token, Is.Not.Null.And.Not.Empty);
        });
    }

}
