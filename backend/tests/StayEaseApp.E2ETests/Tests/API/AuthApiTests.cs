using StayEaseApp.Application.DTOs;
using StayEaseApp.E2ETests.Configuration;
using StayEaseApp.E2ETests.Infrastructure;
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
public class AuthApiTests : ApiTestBase
{
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

        var response = await Request.PostAsync("api/Auth/register", new() { DataObject = request });

        Assert.That(response.Status, Is.EqualTo(200));
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

        var response = await Request.PostAsync("api/Auth/register", new() { DataObject = request });

        Assert.That(response.Status, Is.EqualTo(400));
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

        var response = await Request.PostAsync("api/Auth/login", new() { DataObject = request });

        Assert.That(response.Status, Is.EqualTo(200));
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

        var response = await Request.PostAsync("api/Auth/login", new() { DataObject = request });

        Assert.That(response.Status, Is.EqualTo(401));
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

        var response = await Request.PostAsync("api/Auth/login", new() { DataObject = request });
        var content = await ReadAsAsync<AuthResponseDto>(response);

        Assert.Multiple(() =>
        {
            Assert.That(response.Status, Is.EqualTo(200));
            Assert.That(content?.Token, Is.Not.Null.And.Not.Empty);
        });
    }

}
