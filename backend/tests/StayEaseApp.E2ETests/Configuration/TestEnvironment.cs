using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Configuration;
public class TestEnvironment
{
    public const string BaseUrl = "http://localhost:4200/";

    // Pre-seeded test user expected to exist in the test/staging environment.
    // NOTE: consider replacing with a per-test user created via an API/setup
    // fixture so tests don't depend on external seed data staying in sync.
    public const string ValidUserEmail = "testuser@gmail.com";
    public const string ValidUserPassword = "asd1234";
    public const string ValidUserDisplayName = "T Test user";

    public const string InvalidUserEmail = "does-not-exist@gmail.com";
    public const string InvalidUserPassword = "wrongpassword";
}
