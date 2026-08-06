using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayEaseApp.E2ETests.Configuration;
public class TestEnvironment
{
    public const string BaseUrl = "http://localhost:4200/";

    // valid user to Login to the application, make sure this user exists in the database
    // before running the tests
    public const string ValidUserEmail = "testuser@gmail.com";
    public const string ValidUserPassword = "asd1234";
    public const string ValidUserDisplayName = "T Test user";

    // invalid user to test negative scenarios
    public const string InvalidUserEmail = "does-not-exist@gmail.com";
    public const string InvalidUserPassword = "wrongpassword";

    // valid user to Register to the application, make sure this user does not exist in the database
    // before running the tests
    public const string ValidUserToRegisterFullName = "New User";
    public const string ValidUserToRegisterPassword = "asd1234";
    public const string ValidUserToRegisterConfirmPassword = "asd1234";
    public const string InValidUserToRegisterConfirmPassword = "wrongpassword";
    public const string ValidUserToRegisterDisplayName = "N New User";
}
