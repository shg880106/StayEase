using StayEaseApp.Application.DTOs;
using StayEaseApp.E2ETests.Infrastructure;
using StayEaseApp.E2ETests.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Tests.API.Properties;
[Parallelizable(ParallelScope.Self)]
[TestFixture]
[Category("Api")]
public class GetMyPropertiesApiTests : ApiTestBase
{
    [Test]
    [Category("Properties")]
    public async Task GetMyProperties_WhenUserIsAuthenticated_Returns200Ok()
    {
        var token = await GetAuthTokenAsync(TestUsers.ValidUserEmail, TestUsers.ValidUserPassword);
        
        var response = await Request.GetAsync("api/Property/my-properties", new()
        {
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {token}"
            }
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var properties = await ReadAsAsync<List<PropertyResponseDto>>(response);
        Assert.That(properties, Is.Not.Null);
    }

    [Test]
    [Category("Properties")]
    [Category("NegativePath")]
    public async Task GetMyProperties_WhenUserIsNotAuthenticated_Returns401Unauthorized()
    {
        var response = await Request.GetAsync("api/Property/my-properties");

        Assert.That(response.Status, Is.EqualTo(401));
    }

    [Test]
    [Category("Properties")]
    [Category("NegativePath")]
    public async Task GetMyProperties_WhenTokenIsInvalid_Returns401Unauthorized()
    {
        var response = await Request.GetAsync("api/Property/my-properties", new()
        {
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = "Bearer invalid.token.value"
            }
        });

        Assert.That(response.Status, Is.EqualTo(401));
    }
}
