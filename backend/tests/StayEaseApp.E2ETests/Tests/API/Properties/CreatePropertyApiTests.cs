using Newtonsoft.Json.Linq;
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
public class CreatePropertyApiTests : ApiTestBase
{
    [Test]
    [Category("Properties")]
    public async Task CreateProperty_WhenUserIsAuthenticated_Returns201Created()
    {
        var token = await GetAuthTokenAsync(TestUsers.ValidUserEmail, TestUsers.ValidUserPassword);
        
        var request = new
        {            
            title = "Test title",
            description = "Test description",
            pricePerNight = 120,
            location = "Test location",
            maxGuests = 4,
            imageUrl = ""
        };

        var response = await Request.PostAsync("api/Property", new()
        {
            DataObject = request,
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {token}"
            }
        });

        Assert.That(response.Status, Is.EqualTo(201));

        var property = await ReadAsAsync<PropertyResponseDto>(response);
        Assert.That(property, Is.Not.Null);
        Assert.That(property!.Title, Is.EqualTo(request.title));
    }

    [Test]
    [Category("Properties")]
    [Category("NegativePath")]
    public async Task CreateProperty_WhenUserIsNotAuthenticated_Returns401Unauthorized()
    {        
        var request = new
        {
            title = "Test title",
            description = "Test description",
            pricePerNight = 120,
            location = "Test location",
            maxGuests = 4,
            imageUrl = ""
        };

        var response = await Request.PostAsync("api/Property", new()
        {
            DataObject = request
        });

        Assert.That(response.Status, Is.EqualTo(401));
    }

    [Test]
    [Category("Properties")]
    [Category("NegativePath")]
    public async Task CreateProperty_WhenTokenIsInvalid_Returns401Unauthorized()
    {
        var response = await Request.PostAsync("api/Property", new()
        {
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = "Bearer invalid.token.value"
            }
        });

        Assert.That(response.Status, Is.EqualTo(401));
    }
}
