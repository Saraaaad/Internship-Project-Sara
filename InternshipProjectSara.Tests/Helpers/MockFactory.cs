using Moq;

namespace InternshipProjectSara.Tests.Helpers;

/// <summary>
/// Factory for creating commonly used mock objects.
/// </summary>
public static class MockFactory
{
    /// <summary>
    /// Creates a mock for the specified service type.
    /// </summary>
    public static Mock<T> CreateServiceMock<T>() where T : class
    {
        return new Mock<T>(MockBehavior.Loose);
    }

    /// <summary>
    /// Creates a mock for IAuthorizationService with default settings.
    /// </summary>
    public static Mock<IAuthorizationService> CreateAuthorizationServiceMock(
        bool isAdmin = false,
        bool isHR = false,
        bool canAccessUserData = true,
        int currentUserId = 1)
    {
        var mock = new Mock<IAuthorizationService>();
        
        mock.Setup(x => x.IsAdminOrHR())
            .Returns(isAdmin || isHR);
            
        mock.Setup(x => x.CanAccessUserData(It.IsAny<int>()))
            .Returns(canAccessUserData);
            
        mock.Setup(x => x.GetCurrentUserId())
            .Returns(currentUserId);

        return mock;
    }

    /// <summary>
    /// Creates a mock for IUserService with default empty responses.
    /// </summary>
    public static Mock<IUserService> CreateUserServiceMock()
    {
        var mock = new Mock<IUserService>();
        
        mock.Setup(x => x.GetAll())
            .Returns(new List<UserResponseDto>());
            
        mock.Setup(x => x.GetById(It.IsAny<int>()))
            .Returns((int id) => new UserResponseDto { Id = id });
            
        mock.Setup(x => x.Create(It.IsAny<UserRequestDto>()))
            .Returns((UserRequestDto dto) => new UserResponseDto 
            { 
                Id = 1, 
                FullName = dto.FullName 
            });
            
        mock.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<UserRequestDto>()))
            .Returns((int id, UserRequestDto dto) => new UserResponseDto 
            { 
                Id = id, 
                FullName = dto.FullName 
            });
            
        mock.Setup(x => x.Delete(It.IsAny<int>()));
            
        mock.Setup(x => x.GetByDepartmentId(It.IsAny<int>()))
            .Returns(new List<UserResponseDto>());
            
        mock.Setup(x => x.GetByEmail(It.IsAny<string>()))
            .Returns((string email) => new UserResponseDto { Email = email });
            
        mock.Setup(x => x.GetByUsername(It.IsAny<string>()))
            .Returns((string username) => new UserResponseDto { Username = username });

        return mock;
    }

    /// <summary>
    /// Creates a mock for ITaskService with default empty responses.
    /// </summary>
    public static Mock<ITaskService> CreateTaskServiceMock()
    {
        var mock = new Mock<ITaskService>();
        
        mock.Setup(x => x.GetById(It.IsAny<int>()))
            .Returns((int id) => new TaskResponseDto { Id = id });
            
        mock.Setup(x => x.GetByEmployeeId(It.IsAny<int>()))
            .Returns(new List<TaskResponseDto>());
            
        mock.Setup(x => x.GetByStatus(It.IsAny<Status>()))
            .Returns(new List<TaskResponseDto>());
            
        mock.Setup(x => x.GetByEmployeeAndStatus(It.IsAny<int>(), It.IsAny<Status>()))
            .Returns(new List<TaskResponseDto>());
            
        mock.Setup(x => x.Create(It.IsAny<TaskRequestDto>()))
            .Returns((TaskRequestDto dto) => new TaskResponseDto 
            { 
                Id = 1, 
                Title = dto.Title 
            });
            
        mock.Setup(x => x.UpdateStatus(It.IsAny<int>(), It.IsAny<Status>()))
            .Returns((int id, Status status) => new TaskResponseDto 
            { 
                Id = id, 
                Status = status.ToString()
            });
            
        mock.Setup(x => x.Delete(It.IsAny<int>()));

        return mock;
    }

    /// <summary>
    /// Creates a mock for IDepartmentService with default empty responses.
    /// </summary>
    public static Mock<IDepartmentService> CreateDepartmentServiceMock()
    {
        var mock = new Mock<IDepartmentService>();
        
        mock.Setup(x => x.GetAll())
            .Returns(new List<DepartmentResponseDto>());
            
        mock.Setup(x => x.GetById(It.IsAny<int>()))
            .Returns((int id) => new DepartmentResponseDto { Id = id });
            
        mock.Setup(x => x.Create(It.IsAny<DepartmentRequestDto>()))
            .Returns((DepartmentRequestDto dto) => new DepartmentResponseDto 
            { 
                Id = 1, 
                Name = dto.Name 
            });
            
        mock.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<DepartmentRequestDto>()))
            .Returns((int id, DepartmentRequestDto dto) => new DepartmentResponseDto 
            { 
                Id = id, 
                Name = dto.Name 
            });
            
        mock.Setup(x => x.Delete(It.IsAny<int>()));

        return mock;
    }

    /// <summary>
    /// Creates a mock for INoteService with default empty responses.
    /// </summary>
    public static Mock<INoteService> CreateNoteServiceMock()
    {
        var mock = new Mock<INoteService>();
            
        mock.Setup(x => x.GetById(It.IsAny<int>()))
            .Returns((int id) => new NoteResponseDto { Id = id });
            
        mock.Setup(x => x.GetByEmployeeId(It.IsAny<int>()))
            .Returns(new List<NoteResponseDto>());
            
        mock.Setup(x => x.Create(It.IsAny<NoteRequestDto>()))
            .Returns((NoteRequestDto dto) => new NoteResponseDto 
            { 
                Id = 1, 
                Content = dto.Content 
            });
            
        mock.Setup(x => x.Update(It.IsAny<int>(), It.IsAny<NoteRequestDto>()))
            .Returns((int id, NoteRequestDto dto) => new NoteResponseDto 
            { 
                Id = id, 
                Content = dto.Content 
            });
            
        mock.Setup(x => x.Delete(It.IsAny<int>()));

        return mock;
    }
}

/// <summary>
/// Factory for creating test data objects.
/// </summary>
public static class TestDataFactory
{
    /// <summary>
    /// Creates a test UserRequestDto.
    /// </summary>
    public static UserRequestDto CreateUserRequestDto(
        string? fullName = null,
        string? email = null,
        string? username = null)
    {
        return new UserRequestDto
        {
            FullName = fullName ?? TestDataGenerator.GenerateString(10),
            Email = email ?? TestDataGenerator.GenerateEmail(),
            Username = username ?? TestDataGenerator.GenerateString(8)
        };
    }

    /// <summary>
    /// Creates a test UserResponseDto.
    /// </summary>
    public static UserResponseDto CreateUserResponseDto(
        int? id = null,
        string? fullName = null,
        string? email = null,
        string? username = null)
    {
        return new UserResponseDto
        {
            Id = id ?? TestDataGenerator.GenerateId(),
            FullName = fullName ?? TestDataGenerator.GenerateString(10),
            Email = email ?? TestDataGenerator.GenerateEmail(),
            Username = username ?? TestDataGenerator.GenerateString(8)
        };
    }

    /// <summary>
    /// Creates a test TaskRequestDto.
    /// </summary>
    public static TaskRequestDto CreateTaskRequestDto(
        string? title = null,
        int? employeeId = null)
    {
        return new TaskRequestDto
        {
            Title = title ?? TestDataGenerator.GenerateString(15),
            EmployeeId = employeeId ?? TestDataGenerator.GenerateId()
        };
    }

    /// <summary>
    /// Creates a test TaskResponseDto.
    /// </summary>
    public static TaskResponseDto CreateTaskResponseDto(
        int? id = null,
        string? title = null,
        string? status = null)
    {
        return new TaskResponseDto
        {
            Id = id ?? TestDataGenerator.GenerateId(),
            Title = title ?? TestDataGenerator.GenerateString(15),
            Status = status ?? "Todo"
        };
    }

    /// <summary>
    /// Creates a test DepartmentRequestDto.
    /// </summary>
    public static DepartmentRequestDto CreateDepartmentRequestDto(string? name = null)
    {
        return new DepartmentRequestDto
        {
            Name = name ?? TestDataGenerator.GenerateString(12)
        };
    }

    /// <summary>
    /// Creates a test DepartmentResponseDto.
    /// </summary>
    public static DepartmentResponseDto CreateDepartmentResponseDto(
        int? id = null,
        string? name = null)
    {
        return new DepartmentResponseDto
        {
            Id = id ?? TestDataGenerator.GenerateId(),
            Name = name ?? TestDataGenerator.GenerateString(12)
        };
    }

    /// <summary>
    /// Creates a test NoteRequestDto.
    /// </summary>
    public static NoteRequestDto CreateNoteRequestDto(string? content = null)
    {
        return new NoteRequestDto
        {
            Content = content ?? TestDataGenerator.GenerateString(50)
        };
    }

    /// <summary>
    /// Creates a test NoteResponseDto.
    /// </summary>
    public static NoteResponseDto CreateNoteResponseDto(
        int? id = null,
        string? content = null)
    {
        return new NoteResponseDto
        {
            Id = id ?? TestDataGenerator.GenerateId(),
            Content = content ?? TestDataGenerator.GenerateString(50)
        };
    }
}
