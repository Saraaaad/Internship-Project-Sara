using InternshipProjectSara.Data.Context;

public class UserService : IUserService
{
    private readonly IUserRepository urepository;
    private readonly DatabaseContext _context;

    public UserService(IUserRepository repository, DatabaseContext context)
    {
        urepository = repository;
        _context = context;
    }

    public UserResponseDto GetById(int id)
    {
        var user = urepository.GetById(id);
        if (user == null)
            throw new NotFoundException("User", id);

        return user.ToDto<User, UserResponseDto>();
    }

    public List<UserResponseDto> GetAll()
    {
        var users = urepository.GetAll();
        return users.ToDtoList<User, UserResponseDto>();
    }

    public UserResponseDto Create(UserRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var existingEmail = urepository.GetByEmail(dto.Email);
        if (existingEmail != null)
            throw new DuplicateException($"Email {dto.Email} already exists");

        var existingUser = urepository.GetByUsername(dto.Username);
        if (existingUser != null)
            throw new DuplicateException($"Username {dto.Username} is already taken");

        var user = dto.ToEntity<UserRequestDto, User>();
        urepository.Add(user);
        _context.SaveChanges();
        return user.ToDto<User, UserResponseDto>();
    }

    public UserResponseDto Update(int id, UserRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var user = urepository.GetById(id);
        if (user == null)
            throw new NotFoundException("User", id);

        var transaction = _context.Database.BeginTransaction();

        try
        {
            user.UpdateProfile(dto.FullName, dto.Phone, dto.Email);
            user.ChangeRole(dto.Role);
            user.AssignDepartment(dto.DepartmentId ?? 0);
            user.UpdateSalary(dto.SalaryAmount, dto.SalaryBonus, dto.SalaryCurrency);


            urepository.Update(user);
            _context.SaveChanges();
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
        var user = urepository.GetById(id);
        if (user == null)
            throw new NotFoundException("User", id);

        urepository.Delete(id);
        _context.SaveChanges();
    }

    public UserResponseDto UpdateSalary(int userId, SalaryRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var user = urepository.GetById(userId);
        if (user == null)
            throw new NotFoundException("User", userId);

        if (dto.Amount < 0)
            throw new ValidationException("User", "Salary", dto.Amount.ToString());

        user.UpdateSalary(dto.Amount, dto.Bonus, dto.Currency);

        urepository.Update(user);
        _context.SaveChanges();

        return user.ToDto<User, UserResponseDto>();
    }

    public List<UserResponseDto> GetByDepartmentId(int departmentId)
    {
        var users = urepository.GetByDepartmentId(departmentId);
        if (users == null || users.Count == 0)
            throw new NotFoundException("No users found for the specified department");
            
        return users.ToDtoList<User, UserResponseDto>();
    }

    public UserResponseDto GetByEmail(string email)
    {
        var user = urepository.GetByEmail(email);
        if (user == null)
            throw new NotFoundException("User", email);

        return user.ToDto<User, UserResponseDto>();
    }

    public UserResponseDto GetByUsername(string username)
    {
        var user = urepository.GetByUsername(username);
        if (user == null)
            throw new NotFoundException($"User with username {username} not found");

        return user.ToDto<User, UserResponseDto>();
    }
}