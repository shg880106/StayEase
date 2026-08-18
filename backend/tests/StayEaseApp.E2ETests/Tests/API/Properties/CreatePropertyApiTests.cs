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
            title = "string",
            description = "string",
            pricePerNight = 0,
            location = "string",
            maxGuests = 0,
            imageUrl = "string"
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
}
