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
    //  POST /api/auth/register → 200 con token válido;
    //  400 si email ya existe(espejo de tus tests de UI, pero validando el JSON exacto).
    //	POST /api/auth/login → 200 con token; 401 con credenciales inválidas.
    //	GET /api/auth/me con/sin token → 200 vs 401.
    //	GET /api/property → 200 y forma del payload.

    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        var apiUrl = TestEnvironment.GetApiUrl();
        var isLocal = apiUrl.Contains("localhost");

        var handler = new HttpClientHandler();
        if (isLocal)
        {
            // Solo para entornos locales/dev con certificado autofirmado de Kestrel.
            // No usar este bypass contra entornos productivos o de CI reales.
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

    
}
