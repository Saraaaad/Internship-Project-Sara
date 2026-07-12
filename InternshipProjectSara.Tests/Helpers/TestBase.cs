using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InternshipProjectSara.Tests.Helpers;

/// <summary>
/// Base class for all controller tests providing common setup and utilities.
/// </summary>
public abstract class TestBase : IDisposable
{
    protected MockRepository MockRepository { get; }
    
    protected TestBase()
    {
        MockRepository = new MockRepository(MockBehavior.Strict);
        Setup();
    }

    /// <summary>
    /// Override this method to perform additional setup in derived test classes.
    /// </summary>
    protected virtual void Setup()
    {
    }

    /// <summary>
    /// Creates a mock object with strict behavior.
    /// </summary>
    protected Mock<T> CreateMock<T>() where T : class
    {
        return MockRepository.Create<T>();
    }

    /// <summary>
    /// Verifies all mock expectations were met.
    /// </summary>
    public void Dispose()
    {
        MockRepository.VerifyAll();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Base class for controller tests with common controller setup.
/// </summary>
public abstract class ControllerTestBase<TController> : TestBase where TController : class
{
    protected TController Controller { get; private set; } = default!;

    /// <summary>
    /// Initializes the controller with mocked dependencies.
    /// Override InitializeController in derived classes after setting up mocks.
    /// </summary>
    protected void InitializeController(TController controller)
    {
        Controller = controller;
    }
}

/// <summary>
/// Common test data generators.
/// </summary>
public static class TestDataGenerator
{
    private static readonly Random _random = new();

    public static int GenerateId() => _random.Next(1, 10000);

    public static string GenerateString(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }

    public static string GenerateEmail() => $"{GenerateString(8)}@test.com";

    public static DateTime GenerateDateTime() => DateTime.UtcNow.AddDays(-_random.Next(1, 365));
}

/// <summary>
/// Assertion helper extensions for common test patterns.
/// </summary>
public static class AssertionHelpers
{
    /// <summary>
    /// Asserts that the result is an OkObjectResult with the expected value.
    /// </summary>
    public static T AssertOkResult<T>(this IActionResult result, T expectedValue)
    {
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(expectedValue);
        return (T)okResult.Value!;
    }

    /// <summary>
    /// Asserts that the result is a NotFoundObjectResult.
    /// </summary>
    public static void AssertNotFound(this IActionResult result)
    {
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    /// <summary>
    /// Asserts that the result is a ForbidResult.
    /// </summary>
    public static void AssertForbid(this IActionResult result)
    {
        result.Should().BeOfType<ForbidResult>();
    }

    /// <summary>
    /// Asserts that the result is a StatusCodeResult with the specified status code.
    /// </summary>
    public static void AssertStatusCode(this IActionResult result, int expectedStatusCode)
    {
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(expectedStatusCode);
    }

    /// <summary>
    /// Asserts that the result is a CreatedAtActionResult with the expected action and route values.
    /// </summary>
    public static T AssertCreatedResult<T>(this IActionResult result, string expectedAction)
    {
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(expectedAction);
        return (T)createdResult.Value!;
    }
}
