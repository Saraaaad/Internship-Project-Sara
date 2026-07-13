using InternshipProjectSara.Data.Context;

public class UserService : IUserService
{
    private readonly IUserRepository urepository;
    private readonly IDepartmentRepository drepository;
    private readonly DatabaseContext _context;
    private readonly ILogger<UserService> logger;

    public UserService(IUserRepository repository, IDepartmentRepository departmentRepository, DatabaseContext context, ILogger<UserService> logger)
    {
        urepository = repository;
        drepository = departmentRepository;
        _context = context;
        this.logger = logger;
    }

    public UserResponseDto GetById(int id)
    {
        logger.LogInformation("Retrieving user with ID: {id}", id);
        var user = urepository.GetById(id);
        if (user == null)
        {
            logger.LogWarning("User with ID: {id} not found", id);
            throw new NotFoundException("User", id);
        }
        logger.LogInformation("User with ID: {id} retrieved successfully", id);
        return user.ToDto<User, UserResponseDto>();
    }

    public List<UserResponseDto> GetAll()
    {
        var users = urepository.GetAll();
        if (users == null || users.Count == 0)
            logger.LogInformation("No users found");

        logger.LogInformation("{count} users retrieved successfully", users.Count);
        return users.ToDtoList<User, UserResponseDto>();
    }

    public UserResponseDto Create(UserRequestDto dto)
    {
        logger.LogInformation("Creating user with username: {username} and email: {email}", dto.Username, dto.Email);
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var existingEmail = urepository.GetByEmail(dto.Email);
        if (existingEmail != null)
        {
            logger.LogWarning("Failed, Email {email} already exists", dto.Email);
            throw new DuplicateException($"Email {dto.Email} already exists");
        }
        var existingUser = urepository.GetByUsername(dto.Username);
        if (existingUser != null)
        {
            logger.LogWarning("Failed, Username {username} is already taken", dto.Username);
            throw new DuplicateException($"Username {dto.Username} is already taken");
        }
        var department = drepository.GetById(dto.DepartmentId ?? 0);
        if (department == null)
        {
            logger.LogWarning("Failed, Department with ID {departmentId} not found", dto.DepartmentId ?? 0);
            throw new NotFoundException("Department", dto.DepartmentId ?? 0);
        }
        var user = dto.ToEntity<UserRequestDto, User>();
        urepository.Add(user);
        _context.SaveChanges();

        user.SetSerialNumber(SerialNumberGenerator.Generate(department.DepartmentCode, user.Id));
        _context.SaveChanges();
        logger.LogInformation("User with ID: {id} created successfully", user.Id);
        return user.ToDto<User, UserResponseDto>();
    }

    public UserResponseDto Update(int id, UserRequestDto dto)
    {
        logger.LogInformation("Updating user with ID: {id}", id);
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var user = urepository.GetById(id);
        if (user == null)
        {
            logger.LogWarning("Failed, User with ID: {id} not found", id);
            throw new NotFoundException("User", id);
        }
        var transaction = _context.Database.BeginTransaction();

        try
        {
            user.UpdateProfile(dto.FullName, dto.Phone, dto.Email);
            user.ChangeRole(dto.Role);
            user.AssignDepartment(dto.DepartmentId ?? 0);
            user.UpdateSalary(dto.SalaryAmount, dto.SalaryBonus, dto.SalaryCurrency);


            urepository.Update(user);
            _context.SaveChanges();
            logger.LogInformation("User with ID: {id} updated successfully", user.Id);
            return user.ToDto<User, UserResponseDto>();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public void Delete(int id)
    {
        logger.LogInformation("Deleting user with ID: {id}", id);
        var user = urepository.GetById(id);
        if (user == null)
        {
            logger.LogWarning("Failed, User with ID: {id} not found", id);
            throw new NotFoundException("User", id);
        }
        urepository.Delete(id);
        _context.SaveChanges();
        logger.LogInformation("User with ID: {id} deleted successfully", id);
    }

    public UserResponseDto UpdateSalary(int userId, SalaryRequestDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var user = urepository.GetById(userId);
        if (user == null)
        {
            logger.LogWarning("Failed, User with ID: {userId} not found", userId);
            throw new NotFoundException("User", userId);
        }
        if (dto.Amount < 0)
        {
            logger.LogWarning("Failed, Invalid salary amount for user with ID: {userId}", userId);
            throw new ValidationException("User", "Salary", dto.Amount.ToString());
        }
        user.UpdateSalary(dto.Amount, dto.Bonus, dto.Currency);

        urepository.Update(user);
        _context.SaveChanges();
        logger.LogInformation("Salary for user with ID: {userId} updated successfully", userId);

        return user.ToDto<User, UserResponseDto>();
    }

    public List<UserResponseDto> GetByDepartmentId(int departmentId)
    {
        logger.LogInformation("Retrieving users in department with ID: {departmentId}", departmentId);
        var users = urepository.GetByDepartmentId(departmentId);
        if (drepository.GetById(departmentId) == null)
        {
            logger.LogWarning("Failed, Department with ID: {departmentId} not found", departmentId);
            throw new NotFoundException("Department", departmentId);
        }
        if (users.Count == 0)
            logger.LogInformation("No users found for department with ID: {departmentId} yet", departmentId);

        logger.LogInformation("{count} users retrieved successfully for department with ID: {departmentId}", users.Count, departmentId);
        return users.ToDtoList<User, UserResponseDto>();
    }

    public UserResponseDto GetByEmail(string email)
    {
        logger.LogInformation("Retrieving user with email: {email}", email);
        var user = urepository.GetByEmail(email);
        if (user == null)
        {
            logger.LogWarning("Failed, User with email: {email} not found", email);
            throw new NotFoundException("User", email);
        }
        logger.LogInformation("User with email: {email} retrieved successfully", email);
        return user.ToDto<User, UserResponseDto>();
    }

    public UserResponseDto GetByUsername(string username)
    {
        logger.LogInformation("Retrieving user with username: {username}", username);
        var user = urepository.GetByUsername(username);
        if (user == null){
            logger.LogWarning("Failed, User with username: {username} not found", username);
            throw new NotFoundException("User", username);
        }
        logger.LogInformation("User with username: {username} retrieved successfully", username);
        return user.ToDto<User, UserResponseDto>();
    }
}