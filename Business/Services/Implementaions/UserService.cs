using InternshipProjectSara.Data.Context;

public class UserService : IUserService
{
    private readonly IUserRepository urepository;
    private readonly IDepartmentRepository drepository;
    private readonly DatabaseContext _context;
    private readonly ILogger<UserService> logger;
    private readonly ILogService logService;

    public UserService(IUserRepository repository, IDepartmentRepository departmentRepository, DatabaseContext context, ILogger<UserService> logger, ILogService logService)
    {
        urepository = repository;
        drepository = departmentRepository;
        _context = context;
        this.logger = logger;
        this.logService = logService;
    }

    public UserResponseDto GetById(int id)
    {
        logger.LogInformation("Retrieving user with ID: {id}", id);
        logService.Log(LogLevel.Information, $"Retrieving user with ID: {id}");
        var user = urepository.GetById(id);
        if (user == null)
        {
            logger.LogWarning("User with ID: {id} not found", id);
            logService.Log(LogLevel.Warning, $"User with ID: {id} not found");
            throw new NotFoundException("User", id);
        }
        logger.LogInformation("User with ID: {id} retrieved successfully", id);
        logService.Log(LogLevel.Information, $"User with ID: {id} retrieved successfully");
        return user.ToDto<User, UserResponseDto>();
    }

    public List<UserResponseDto> GetAll()
    {
        var users = urepository.GetAll();
        if (users == null || users.Count == 0)
        {
            logger.LogInformation("No users found");
            logService.Log(LogLevel.Information, "No users found");
        }
        logger.LogInformation("{count} users retrieved successfully", users.Count);
        logService.Log(LogLevel.Information, $"{users.Count} users retrieved successfully");
        return users.ToDtoList<User, UserResponseDto>();
    }

    public UserResponseDto Create(UserRequestDto dto)
    {
        logService.Log(LogLevel.Information, $"Creating user with username: {dto.Username} and email: {dto.Email}");
        logger.LogInformation("Creating user with username: {username} and email: {email}", dto.Username, dto.Email);
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var existingEmail = urepository.GetByEmail(dto.Email);
        if (existingEmail != null)
        {
            logger.LogWarning("Failed, Email {email} already exists", dto.Email);
            logService.Log(LogLevel.Warning, $"Email {dto.Email} already exists");
            throw new DuplicateException($"Email {dto.Email} already exists");
        }
        var existingUser = urepository.GetByUsername(dto.Username);
        if (existingUser != null)
        {
            logger.LogWarning("Failed, Username {username} is already taken", dto.Username);
            logService.Log(LogLevel.Warning, $"Username {dto.Username} is already taken");
            throw new DuplicateException($"Username {dto.Username} is already taken");
        }
        var department = drepository.GetById(dto.DepartmentId ?? 0);
        if (department == null)
        {
            logger.LogWarning("Failed, Department with ID {departmentId} not found", dto.DepartmentId ?? 0);
            logService.Log(LogLevel.Warning, $"Department with ID {dto.DepartmentId ?? 0} not found");
            throw new NotFoundException("Department", dto.DepartmentId ?? 0);
        }
        var user = dto.ToEntity<UserRequestDto, User>();
        urepository.Add(user);
        _context.SaveChanges();

        user.SetSerialNumber(SerialNumberGenerator.Generate(department.DepartmentCode, user.Id));
        _context.SaveChanges();
        logger.LogInformation("User with ID: {id} created successfully", user.Id);
        logService.Log(LogLevel.Information, $"User with ID: {user.Id} created successfully");
        return user.ToDto<User, UserResponseDto>();
    }

    public UserResponseDto Update(int id, UserUpdateDto dto)
    {
        logger.LogInformation("Updating user with ID: {id}", id);
        logService.Log(LogLevel.Information, $"Updating user with ID: {id}");
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var user = urepository.GetById(id);
        if (user == null)
        {
            logger.LogWarning("Failed, User with ID: {id} not found", id);
            logService.Log(LogLevel.Warning, $"User with ID: {id} not found");
            throw new NotFoundException("User", id);
        }
        user.UpdateProfile(dto.FullName, dto.Email, dto.Phone);
        if (dto.DepartmentId.HasValue)
        {
            user.AssignDepartment(dto.DepartmentId.Value);
            if (string.IsNullOrEmpty(user.SerialNumber))
            {
                var department = drepository.GetById(dto.DepartmentId.Value);
                if (department == null)
                    throw new NotFoundException("Department", dto.DepartmentId.Value);

                user.SetSerialNumber(
                    SerialNumberGenerator.Generate(department.DepartmentCode, user.Id));
            }
        }

        urepository.Update(user);
        _context.SaveChanges();
        logger.LogInformation("User with ID: {id} updated successfully", user.Id);
        logService.Log(LogLevel.Information, $"User with ID: {user.Id} updated successfully");
        return user.ToDto<User, UserResponseDto>();
    }

    public void Delete(int id)
    {
        logger.LogInformation("Deleting user with ID: {id}", id);
        logService.Log(LogLevel.Information, $"Deleting user with ID: {id}");
        var user = urepository.GetById(id);
        if (user == null)
        {
            logger.LogWarning("Failed, User with ID: {id} not found", id);
            logService.Log(LogLevel.Warning, $"User with ID: {id} not found");
            throw new NotFoundException("User", id);
        }
        var transaction = _context.Database.BeginTransaction();
        try
        {
            var notes = _context.Notes.Where(n => n.EmployeeId == id);
            _context.Notes.RemoveRange(notes);
            var tasks = _context.Tasks.Where(t => t.EmployeeId == id);
            _context.Tasks.RemoveRange(tasks);

            urepository.Delete(id);
            _context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
        logger.LogInformation("User with ID: {id} deleted successfully", id);
        logService.Log(LogLevel.Information, $"User with ID: {id} deleted successfully");
    }

    public UserResponseDto UpdateSalary(int userId, SalaryRequestDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var user = urepository.GetById(userId);
        if (user == null)
        {
            logger.LogWarning("Failed, User with ID: {userId} not found", userId);
            logService.Log(LogLevel.Warning, $"User with ID: {userId} not found");
            throw new NotFoundException("User", userId);
        }
        if (dto.Amount < 0)
        {
            logger.LogWarning("Failed, Invalid salary amount for user with ID: {userId}", userId);
            logService.Log(LogLevel.Warning, $"Invalid salary amount for user with ID: {userId}");
            throw new ValidationException("User", "Salary", dto.Amount.ToString());
        }
        user.UpdateSalary(dto.Amount, dto.Bonus, dto.Currency);

        urepository.Update(user);
        _context.SaveChanges();
        logger.LogInformation("Salary for user with ID: {userId} updated successfully", userId);
        logService.Log(LogLevel.Information, $"Salary for user with ID: {userId} updated successfully");

        return user.ToDto<User, UserResponseDto>();
    }

    public List<UserResponseDto> GetByDepartmentId(int departmentId)
    {
        logger.LogInformation("Retrieving users in department with ID: {departmentId}", departmentId);
        logService.Log(LogLevel.Information, $"Retrieving users in department with ID: {departmentId}");
        var users = urepository.GetByDepartmentId(departmentId);
        if (drepository.GetById(departmentId) == null)
        {
            logger.LogWarning("Failed, Department with ID: {departmentId} not found", departmentId);
            logService.Log(LogLevel.Warning, $"Department with ID: {departmentId} not found");
            throw new NotFoundException("Department", departmentId);
        }
        if (users.Count == 0)
        {
            logService.Log(LogLevel.Warning, $"No users found for department with ID: {departmentId} yet");
            logger.LogInformation("No users found for department with ID: {departmentId} yet", departmentId);
        }
        logger.LogInformation("{count} users retrieved successfully for department with ID: {departmentId}", users.Count, departmentId);
        logService.Log(LogLevel.Information, $"{users.Count} users retrieved successfully for department with ID: {departmentId}");
        return users.ToDtoList<User, UserResponseDto>();
    }

    public UserResponseDto GetByEmail(string email)
    {
        logger.LogInformation("Retrieving user with email: {email}", email);
        logService.Log(LogLevel.Information, $"Retrieving user with email: {email}");
        var user = urepository.GetByEmail(email);
        if (user == null)
        {
            logger.LogWarning("Failed, User with email: {email} not found", email);
            logService.Log(LogLevel.Warning, $"User with email: {email} not found");
            throw new NotFoundException("User", email);
        }
        logger.LogInformation("User with email: {email} retrieved successfully", email);
        logService.Log(LogLevel.Information, $"User with email: {email} retrieved successfully");
        return user.ToDto<User, UserResponseDto>();
    }

    public UserResponseDto GetByUsername(string username)
    {
        logger.LogInformation("Retrieving user with username: {username}", username);
        logService.Log(LogLevel.Information, $"Retrieving user with username: {username}");
        var user = urepository.GetByUsername(username);
        if (user == null)
        {
            logger.LogWarning("Failed, User with username: {username} not found", username);
            logService.Log(LogLevel.Warning, $"User with username: {username} not found");
            throw new NotFoundException("User", username);
        }
        logger.LogInformation("User with username: {username} retrieved successfully", username);
        logService.Log(LogLevel.Information, $"User with username: {username} retrieved successfully");
        return user.ToDto<User, UserResponseDto>();
    }
    public void ChangeRole(int userId, RoleChangeDto dto)
    {
        logger.LogInformation("Changing role for user with ID: {userId} to {role}", userId, dto.Role);
        logService.Log(LogLevel.Information, $"Changing role for user with ID: {userId} to {dto.Role}");
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));
        var user = urepository.GetById(userId);
        if (user == null)
        {
            logger.LogWarning("Failed, User with ID: {userId} not found", userId);
            logService.Log(LogLevel.Warning, $"User with ID: {userId} not found");
            throw new NotFoundException("User", userId);
        }
        user.ChangeRole(dto.Role);
        urepository.Update(user);
        _context.SaveChanges();
        logger.LogInformation("Role for user with ID: {userId} changed successfully to {role}", userId, dto.Role);
        logService.Log(LogLevel.Information, $"Role for user with ID: {userId} changed successfully to {dto.Role}");
    }
}