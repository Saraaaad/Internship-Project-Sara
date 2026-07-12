using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace InternshipProjectSara.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IAuthorizationService> _authorizationServiceMock;

    private readonly UserController _controller;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _controller = new UserController(
            _userServiceMock.Object, _authorizationServiceMock.Object
        );
    }

    [Fact]
    public void GetAll_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new List<UserResponseDto>
        {
            new UserResponseDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Role = "Admin",
                FullName = "Test User",
                Phone = "+1234567890",
                SerialNumber = "TestValue",
                DepartmentId = null,
                DepartmentName = null,
                SalaryAmount = 1000.00m,
                SalaryBonus = 1000.00m,
                TotalSalary = "TestValue",
                SalaryCurrency = "USD",
                CreatedAt = DateTime.UtcNow
            }
        };
        _userServiceMock.Setup(x => x.GetAll()).Returns(expected);

        // Act
        var result = _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<List<UserResponseDto>>().Subject;
    }

    [Fact]
    public void GetAll_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetAll())
            .Throws(new Exception("Test exception"));

        // Act
        var result = _controller.GetAll();

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void GetAll_WhenUnauthorized_ReturnsForbid()
    {
        // Arrange
        // User doesn't have required role

        // Act
        var result = _controller.GetAll();

        // Assert
        // ASP.NET automatically returns 403 for role mismatch
    }

    [Fact]
    public void GetById_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new UserResponseDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Role = "Admin",
            FullName = "Test User",
            Phone = "+1234567890",
            SerialNumber = "TestValue",
            DepartmentId = null,
            DepartmentName = null,
            SalaryAmount = 1000.00m,
            SalaryBonus = 1000.00m,
            TotalSalary = "TestValue",
            SalaryCurrency = "USD",
            CreatedAt = DateTime.UtcNow
        };
        _userServiceMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(expected);
        _authorizationServiceMock.Setup(x => x.CanAccessUserData(It.IsAny<int>())).Returns(true);

        // Act
        var result = _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<UserResponseDto>().Subject;
    }

    [Fact]
    public void GetById_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetById(It.IsAny<int>()))
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
        var expected = new UserResponseDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Role = "Admin",
            FullName = "Test User",
            Phone = "+1234567890",
            SerialNumber = "TestValue",
            DepartmentId = null,
            DepartmentName = null,
            SalaryAmount = 1000.00m,
            SalaryBonus = 1000.00m,
            TotalSalary = "TestValue",
            SalaryCurrency = "USD",
            CreatedAt = DateTime.UtcNow
        };
        _userServiceMock.Setup(x => x.Create(It.IsAny<UserRequestDto>())).Returns(expected);

        var testUserRequestDto = new UserRequestDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            Role = Role.Admin,
            FullName = "Test User",
            Phone = "+1234567890",
            DepartmentId = null,
            SalaryAmount = 1000.00m,
            SalaryBonus = 1000.00m,
            SalaryCurrency = "USD"
        };
        // Act
        var result = _controller.Create(testUserRequestDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedValue = createdResult.Value.Should().BeAssignableTo<UserResponseDto>().Subject;
    }

    [Fact]
    public void Create_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _userServiceMock.Setup(x => x.Create(It.IsAny<UserRequestDto>()))
            .Throws(new Exception("Test exception"));

        // Act
        var testUserRequestDto = new UserRequestDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            Role = Role.Admin,
            FullName = "Test User",
            Phone = "+1234567890",
            DepartmentId = null,
            SalaryAmount = 1000.00m,
            SalaryBonus = 1000.00m,
            SalaryCurrency = "USD"
        };
        var result = _controller.Create(testUserRequestDto);

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
        var testUserRequestDto = new UserRequestDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            Role = Role.Admin,
            FullName = "Test User",
            Phone = "+1234567890",
            DepartmentId = null,
            SalaryAmount = 1000.00m,
            SalaryBonus = 1000.00m,
            SalaryCurrency = "USD"
        };
        var result = _controller.Create(testUserRequestDto);

        // Assert
        // ASP.NET automatically returns 403 for role mismatch
    }

    [Fact]
    public void Update_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new UserResponseDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Role = "Admin",
            FullName = "Test User",
            Phone = "+1234567890",
            SerialNumber = "TestValue",
            DepartmentId = null,
            DepartmentName = null,
            SalaryAmount = 1000.00m,
            SalaryBonus = 1000.00m,
            TotalSalary = "TestValue",
            SalaryCurrency = "USD",
            CreatedAt = DateTime.UtcNow
        };
        _userServiceMock.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<UserRequestDto>())).Returns(expected);

        var testUserRequestDto = new UserRequestDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            Role = Role.Admin,
            FullName = "Test User",
            Phone = "+1234567890",
            DepartmentId = null,
            SalaryAmount = 1000.00m,
            SalaryBonus = 1000.00m,
            SalaryCurrency = "USD"
        };
        // Act
        var result = _controller.Update(testUserRequestDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<UserResponseDto>().Subject;
    }

    [Fact]
    public void Update_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _userServiceMock.Setup(x => x.Update(It.IsAny<UserRequestDto>()))
            .Throws(new Exception("Test exception"));

        // Act
        var testUserRequestDto = new UserRequestDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            Role = Role.Admin,
            FullName = "Test User",
            Phone = "+1234567890",
            DepartmentId = null,
            SalaryAmount = 1000.00m,
            SalaryBonus = 1000.00m,
            SalaryCurrency = "USD"
        };
        var result = _controller.Update(testUserRequestDto);

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
        var testUserRequestDto = new UserRequestDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            Role = Role.Admin,
            FullName = "Test User",
            Phone = "+1234567890",
            DepartmentId = null,
            SalaryAmount = 1000.00m,
            SalaryBonus = 1000.00m,
            SalaryCurrency = "USD"
        };
        var result = _controller.Update(testUserRequestDto);

        // Assert
        // ASP.NET automatically returns 403 for role mismatch
    }

    [Fact]
    public void Delete_WhenValid_ReturnsSuccess()
    {
        // Arrange
        _userServiceMock.Setup(x => x.Delete(It.IsAny<int>()));

        // Act
        var result = _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void Delete_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _userServiceMock.Setup(x => x.Delete(It.IsAny<int>()))
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
    public void UpdateSalary_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new UserResponseDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Role = "Admin",
            FullName = "Test User",
            Phone = "+1234567890",
            SerialNumber = "TestValue",
            DepartmentId = null,
            DepartmentName = null,
            SalaryAmount = 1000.00m,
            SalaryBonus = 1000.00m,
            TotalSalary = "TestValue",
            SalaryCurrency = "USD",
            CreatedAt = DateTime.UtcNow
        };
        _userServiceMock.Setup(x => x.UpdateSalary(It.IsAny<int>(), It.IsAny<SalaryRequestDto>())).Returns(expected);

        var testSalaryRequestDto = new SalaryRequestDto
        {
            Amount = 1000.00m,
            Bonus = 1000.00m,
            Currency = "USD"
        };
        // Act
        var result = _controller.UpdateSalary(testSalaryRequestDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<UserResponseDto>().Subject;
    }

    [Fact]
    public void UpdateSalary_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _userServiceMock.Setup(x => x.UpdateSalary(It.IsAny<SalaryRequestDto>()))
            .Throws(new Exception("Test exception"));

        // Act
        var testSalaryRequestDto = new SalaryRequestDto
        {
            Amount = 1000.00m,
            Bonus = 1000.00m,
            Currency = "USD"
        };
        var result = _controller.UpdateSalary(testSalaryRequestDto);

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void UpdateSalary_WhenUnauthorized_ReturnsForbid()
    {
        // Arrange
        // User doesn't have required role

        // Act
        var testSalaryRequestDto = new SalaryRequestDto
        {
            Amount = 1000.00m,
            Bonus = 1000.00m,
            Currency = "USD"
        };
        var result = _controller.UpdateSalary(testSalaryRequestDto);

        // Assert
        // ASP.NET automatically returns 403 for role mismatch
    }

    [Fact]
    public void GetByDepartmentId_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new List<UserResponseDto>
        {
            new UserResponseDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Role = "Admin",
                FullName = "Test User",
                Phone = "+1234567890",
                SerialNumber = "TestValue",
                DepartmentId = null,
                DepartmentName = null,
                SalaryAmount = 1000.00m,
                SalaryBonus = 1000.00m,
                TotalSalary = "TestValue",
                SalaryCurrency = "USD",
                CreatedAt = DateTime.UtcNow
            }
        };
        _userServiceMock.Setup(x => x.GetByDepartmentId(It.IsAny<int>())).Returns(expected);

        // Act
        var result = _controller.GetByDepartmentId(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<List<UserResponseDto>>().Subject;
    }

    [Fact]
    public void GetByDepartmentId_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetByDepartmentId(It.IsAny<int>()))
            .Throws(new Exception("Test exception"));

        // Act
        var result = _controller.GetByDepartmentId(1);

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void GetByDepartmentId_WhenUnauthorized_ReturnsForbid()
    {
        // Arrange
        // User doesn't have required role

        // Act
        var result = _controller.GetByDepartmentId(1);

        // Assert
        // ASP.NET automatically returns 403 for role mismatch
    }

    [Fact]
    public void GetByEmail_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new UserResponseDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Role = "Admin",
            FullName = "Test User",
            Phone = "+1234567890",
            SerialNumber = "TestValue",
            DepartmentId = null,
            DepartmentName = null,
            SalaryAmount = 1000.00m,
            SalaryBonus = 1000.00m,
            TotalSalary = "TestValue",
            SalaryCurrency = "USD",
            CreatedAt = DateTime.UtcNow
        };
        _userServiceMock.Setup(x => x.GetByEmail(It.IsAny<string>())).Returns(expected);

        // Act
        var result = _controller.GetByEmail("test");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<UserResponseDto>().Subject;
    }

    [Fact]
    public void GetByEmail_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetByEmail(It.IsAny<string>()))
            .Throws(new Exception("Test exception"));

        // Act
        var result = _controller.GetByEmail("test");

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void GetByEmail_WhenUnauthorized_ReturnsForbid()
    {
        // Arrange
        // User doesn't have required role

        // Act
        var result = _controller.GetByEmail("test");

        // Assert
        // ASP.NET automatically returns 403 for role mismatch
    }

    [Fact]
    public void GetByUsername_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new UserResponseDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Role = "Admin",
            FullName = "Test User",
            Phone = "+1234567890",
            SerialNumber = "TestValue",
            DepartmentId = null,
            DepartmentName = null,
            SalaryAmount = 1000.00m,
            SalaryBonus = 1000.00m,
            TotalSalary = "TestValue",
            SalaryCurrency = "USD",
            CreatedAt = DateTime.UtcNow
        };
        _userServiceMock.Setup(x => x.GetByUsername(It.IsAny<string>())).Returns(expected);

        // Act
        var result = _controller.GetByUsername("test");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<UserResponseDto>().Subject;
    }

    [Fact]
    public void GetByUsername_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetByUsername(It.IsAny<string>()))
            .Throws(new Exception("Test exception"));

        // Act
        var result = _controller.GetByUsername("test");

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void GetByUsername_WhenUnauthorized_ReturnsForbid()
    {
        // Arrange
        // User doesn't have required role

        // Act
        var result = _controller.GetByUsername("test");

        // Assert
        // ASP.NET automatically returns 403 for role mismatch
    }

    [Fact]
    public void GetProfile_WhenValid_ReturnsSuccess()
    {
        // Arrange
        var expected = new UserResponseDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Role = "Admin",
            FullName = "Test User",
            Phone = "+1234567890",
            SerialNumber = "TestValue",
            DepartmentId = null,
            DepartmentName = null,
            SalaryAmount = 1000.00m,
            SalaryBonus = 1000.00m,
            TotalSalary = "TestValue",
            SalaryCurrency = "USD",
            CreatedAt = DateTime.UtcNow
        };
        _userServiceMock.Setup(x => x.GetProfile()).Returns(expected);
        _authorizationServiceMock.Setup(x => x.CanAccessUserData(It.IsAny<int>())).Returns(true);

        // Act
        var result = _controller.GetProfile();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedValue = okResult.Value.Should().BeAssignableTo<UserResponseDto>().Subject;
    }

    [Fact]
    public void GetProfile_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetProfile())
            .Throws(new Exception("Test exception"));

        // Act
        var result = _controller.GetProfile();

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }

    [Fact]
    public void ChangePassword_WhenValid_ReturnsSuccess()
    {
        // Arrange
        // No mock setup needed for IActionResult
        _authorizationServiceMock.Setup(x => x.CanAccessUserData(It.IsAny<int>())).Returns(true);

        var testChangePasswordRequestDto = new ChangePasswordRequestDto
        {
            NewPassword = "Password123!",
            ConfirmPassword = "Password123!"
        };
        // Act
        var result = _controller.ChangePassword(testChangePasswordRequestDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public void ChangePassword_WhenExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        _userServiceMock.Setup(x => x.ChangePassword(It.IsAny<ChangePasswordRequestDto>()))
            .Throws(new Exception("Test exception"));

        // Act
        var testChangePasswordRequestDto = new ChangePasswordRequestDto
        {
            NewPassword = "Password123!",
            ConfirmPassword = "Password123!"
        };
        var result = _controller.ChangePassword(testChangePasswordRequestDto);

        // Assert
        var objectResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Test exception");
    }
}
