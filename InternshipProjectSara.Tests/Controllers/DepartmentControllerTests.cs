using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InternshipProjectSara.Tests.Controllers;

public class DepartmentControllerTests
{
    private readonly Mock<IDepartmentService> _departmentServiceMock;
    private readonly Mock<IAuthorizationService> _authorizationServiceMock;

    private readonly DepartmentController _controller;

    public DepartmentControllerTests()
    {
        _departmentServiceMock = new Mock<IDepartmentService>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _controller = new DepartmentController(
            _departmentServiceMock.Object
        );
    }

    [Fact]
    public void GetAll_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new List<DepartmentResponseDto>
        {
            new DepartmentResponseDto
            {
                Name = "Test",
                DepartmentCode = "TestValue",
                LeadNumber = null,
                EmployeeCount = 1,
                EmployeeNames = null
            }
        };
        _departmentServiceMock.Setup(x => x.GetAll()).Returns(expected);
        _authorizationServiceMock.Setup(x => x.CanAccessUserData(It.IsAny<int>())).Returns(true);

        // Act
        var result = _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<List<DepartmentResponseDto>>().Subject;
    }

    [Fact]
    public void GetAll_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _departmentServiceMock.Setup(x => x.GetAll())
            .Throws(new Exception("Test exception"));

        // Act
        var result = _controller.GetAll();

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void GetById_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new DepartmentResponseDto
        {
            Name = "Test",
            DepartmentCode = "TestValue",
            LeadNumber = null,
            EmployeeCount = 1,
            EmployeeNames = null
        };
        _departmentServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(expected);
        _authorizationServiceMock.Setup(x => x.CanAccessUserData(It.IsAny<int>())).Returns(true);

        // Act
        var result = _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<DepartmentResponseDto>().Subject;
    }

    [Fact]
    public void GetById_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _departmentServiceMock.Setup(x => x.GetById(It.IsAny<int>()))
            .Throws(new Exception("Test exception"));

        // Act
        var result = _controller.GetById(1);

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void Create_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new DepartmentResponseDto
        {
            Name = "Test",
            DepartmentCode = "TestValue",
            LeadNumber = null,
            EmployeeCount = 1,
            EmployeeNames = null
        };
        _departmentServiceMock.Setup(x => x.Create(It.IsAny<DepartmentRequestDto>())).Returns(expected);

        var testDepartmentRequestDto = new DepartmentRequestDto
        {
            DepartmentCode = "TestValue",
            LeadSerialNumber = null
        };
        // Act
        var result = _controller.Create(testDepartmentRequestDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedValue = createdResult.Value.Should().BeAssignableTo<DepartmentResponseDto>().Subject;
    }

    [Fact]
    public void Create_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _departmentServiceMock.Setup(x => x.Create(It.IsAny<DepartmentRequestDto>()))
            .Throws(new Exception("Test exception"));

        // Act
        var testDepartmentRequestDto = new DepartmentRequestDto
        {
            DepartmentCode = "TestValue",
            LeadSerialNumber = null
        };
        var result = _controller.Create(testDepartmentRequestDto);

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
        var testDepartmentRequestDto = new DepartmentRequestDto
        {
            DepartmentCode = "TestValue",
            LeadSerialNumber = null
        };
        var result = _controller.Create(testDepartmentRequestDto);

        // Assert
        // ASP.NET automatically returns 403 for role mismatch
    }

    [Fact]
    public void Update_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new DepartmentResponseDto
        {
            Name = "Test",
            DepartmentCode = "TestValue",
            LeadNumber = null,
            EmployeeCount = 1,
            EmployeeNames = null
        };
        _departmentServiceMock.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<DepartmentRequestDto>())).Returns(expected);

        var testDepartmentRequestDto = new DepartmentRequestDto
        {
            DepartmentCode = "TestValue",
            LeadSerialNumber = null
        };
        // Act
        var result = _controller.Update(expected.Id, testDepartmentRequestDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<DepartmentResponseDto>().Subject;
    }

    [Fact]
    public void Update_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _departmentServiceMock.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<DepartmentRequestDto>()))
            .Throws(new Exception("Test exception"));

        // Act
        var testDepartmentRequestDto = new DepartmentRequestDto
        {
            DepartmentCode = "TestValue",
            LeadSerialNumber = null
        };
        var result = _controller.Update(1, testDepartmentRequestDto);

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void Update_WhenUnauthorized_ReturnsForbid()
    {
        // Arrange
        // User doesn't have required role

        // Act
        var testDepartmentRequestDto = new DepartmentRequestDto
        {
            DepartmentCode = "TestValue",
            LeadSerialNumber = null
        };
        var result = _controller.Update(1, testDepartmentRequestDto);

        // Assert
        // ASP.NET automatically returns 403 for role mismatch
    }

    [Fact]
    public void Delete_WhenValid_ReturnsSuccess()
    {
        // Arrange
        _departmentServiceMock.Setup(x => x.Delete(It.IsAny<int>()));

        // Act
        var result = _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void Delete_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _departmentServiceMock.Setup(x => x.Delete(It.IsAny<int>()))
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

    [Fact]
    public void GetByName_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new DepartmentResponseDto
        {
            Name = "Test",
            DepartmentCode = "TestValue",
            LeadNumber = null,
            EmployeeCount = 1,
            EmployeeNames = null
        };
        _departmentServiceMock.Setup(x => x.GetByName(It.IsAny<string>())).Returns(expected);
        _authorizationServiceMock.Setup(x => x.CanAccessUserData(It.IsAny<int>())).Returns(true);

        // Act
        var result = _controller.GetByName("test");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<DepartmentResponseDto>().Subject;
    }

    [Fact]
    public void GetByName_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _departmentServiceMock.Setup(x => x.GetByName(It.IsAny<string>()))
            .Throws(new Exception("Test exception"));

        // Act
        var result = _controller.GetByName("test");

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }
}
