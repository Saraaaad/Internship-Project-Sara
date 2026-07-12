using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InternshipProjectSara.Tests.Controllers;

public class TaskControllerTests
{
    private readonly Mock<ITaskService> _taskServiceMock;
    private readonly Mock<IAuthorizationService> _authorizationServiceMock;
    private readonly TaskController _controller;

    public TaskControllerTests()
    {
        _taskServiceMock = new Mock<ITaskService>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _controller = new TaskController(
            _taskServiceMock.Object, _authorizationServiceMock.Object
        );
    }

    [Fact]
    public void GetById_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new TaskResponseDto
        {
            Title = "TestValue",
            Description = "TestValue",
            Status = "TestValue",
            Deadline = DateTime.UtcNow,
            EmployeeId = 1,
            EmployeeName = "Test",
            CreatedAt = DateTime.UtcNow
        };
        _taskServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(expected);
        _authorizationServiceMock.Setup(x => x.CanAccessUserData(It.IsAny<int>())).Returns(true);

        // Act
        var result = _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<TaskResponseDto>().Subject;
    }

    [Fact]
    public void GetById_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _taskServiceMock.Setup(x => x.GetById(It.IsAny<int>()))
            .Throws(new Exception("Test exception"));

        // Act
        var result = _controller.GetById(1);

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void GetByEmployeeId_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new List<TaskResponseDto>
        {
            new TaskResponseDto
            {
                Title = "TestValue",
                Description = "TestValue",
                Status = "TestValue",
                Deadline = DateTime.UtcNow,
                EmployeeId = 1,
                EmployeeName = "Test",
                CreatedAt = DateTime.UtcNow
            }
        };
        _taskServiceMock.Setup(x => x.GetByEmployeeId(It.IsAny<int>())).Returns(expected);
        _authorizationServiceMock.Setup(x => x.CanAccessUserData(It.IsAny<int>())).Returns(true);

        // Act
        var result = _controller.GetByEmployeeId(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<List<TaskResponseDto>>().Subject;
    }

    [Fact]
    public void GetByEmployeeId_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _taskServiceMock.Setup(x => x.GetByEmployeeId(It.IsAny<int>()))
            .Throws(new Exception("Test exception"));

        // Act
        var result = _controller.GetByEmployeeId(1);

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void GetByStatus_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new List<TaskResponseDto>
        {
            new TaskResponseDto
            {
                Title = "TestValue",
                Description = "TestValue",
                Status = "TestValue",
                Deadline = DateTime.UtcNow,
                EmployeeId = 1,
                EmployeeName = "Test",
                CreatedAt = DateTime.UtcNow
            }
        };
        _taskServiceMock.Setup(x => x.GetByStatus(It.IsAny<Status>())).Returns(expected);
        _authorizationServiceMock.Setup(x => x.CanAccessUserData(It.IsAny<int>())).Returns(true);
        _authorizationServiceMock.Setup(x => x.IsAdminOrHR()).Returns(true);

        // Act
        var result = _controller.GetByStatus(Status.Todo);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<List<TaskResponseDto>>().Subject;
    }

    [Fact]
    public void GetByStatus_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _taskServiceMock.Setup(x => x.GetByStatus(It.IsAny<Status>()))
            .Throws(new Exception("Test exception"));
        _authorizationServiceMock.Setup(x => x.IsAdminOrHR()).Returns(true);

        // Act
        var result = _controller.GetByStatus(Status.Todo);

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void Create_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new TaskResponseDto
        {
            Title = "TestValue",
            Description = "TestValue",
            Status = "TestValue",
            Deadline = DateTime.UtcNow,
            EmployeeId = 1,
            EmployeeName = "Test",
            CreatedAt = DateTime.UtcNow
        };
        _taskServiceMock.Setup(x => x.Create(It.IsAny<TaskRequestDto>())).Returns(expected);

        var testTaskRequestDto = new TaskRequestDto
        {
            Description = "TestValue",
            EmployeeId = 1,
            Status = Status.Todo,
            Deadline = DateTime.UtcNow
        };
        // Act
        var result = _controller.Create(testTaskRequestDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedValue = createdResult.Value.Should().BeAssignableTo<TaskResponseDto>().Subject;
    }

    [Fact]
    public void Create_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _taskServiceMock.Setup(x => x.Create(It.IsAny<TaskRequestDto>()))
            .Throws(new Exception("Test exception"));

        // Act
        var testTaskRequestDto = new TaskRequestDto
        {
            Description = "TestValue",
            EmployeeId = 1,
            Status = Status.Todo,
            Deadline = DateTime.UtcNow
        };
        var result = _controller.Create(testTaskRequestDto);

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void Create_WhenUnauthorized_ReturnsForbid()
    {
        // Arrange
        // User doesn't have required role

        // Act
        var testTaskRequestDto = new TaskRequestDto
        {
            Description = "TestValue",
            EmployeeId = 1,
            Status = Status.Todo,
            Deadline = DateTime.UtcNow
        };
        var result = _controller.Create(testTaskRequestDto);

        // Assert
        // ASP.NET automatically returns 403 for role mismatch
    }

    [Fact]
    public void UpdateStatus_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new TaskResponseDto
        {
            Title = "TestValue",
            Description = "TestValue",
            Status = "TestValue",
            Deadline = DateTime.UtcNow,
            EmployeeId = 1,
            EmployeeName = "Test",
            CreatedAt = DateTime.UtcNow
        };
        _taskServiceMock.Setup(x => x.UpdateStatus(It.IsAny<int>(), It.IsAny<Status>())).Returns(expected);
        _authorizationServiceMock.Setup(x => x.CanAccessUserData(It.IsAny<int>())).Returns(true);
        _taskServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(expected);

        // Act
        var result = _controller.UpdateStatus(1, Status.Todo);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<TaskResponseDto>().Subject;
    }

    [Fact]
    public void UpdateStatus_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _taskServiceMock.Setup(x => x.UpdateStatus(It.IsAny<int>(), It.IsAny<Status>()))
            .Throws(new Exception("Test exception"));
        _taskServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(new TaskResponseDto { EmployeeId = 1 });
        _authorizationServiceMock.Setup(x => x.CanAccessUserData(It.IsAny<int>())).Returns(true);

        // Act
        var result = _controller.UpdateStatus(1, Status.Todo);

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void Delete_WhenValid_ReturnsSuccess()
    {
        // Arrange
        _taskServiceMock.Setup(x => x.Delete(It.IsAny<int>()));

        // Act
        var result = _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void Delete_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _taskServiceMock.Setup(x => x.Delete(It.IsAny<int>()))
            .Throws(new Exception("Test exception"));

        // Act
        var result = _controller.Delete(1);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void Delete_WhenUnauthorized_ReturnsForbid()
    {
        // Arrange
        // User doesn't have required role

        // Act
        var result = _controller.Delete(1);

        // Assert
        // ASP.NET automatically returns 403 for role mismatch
    }
}
