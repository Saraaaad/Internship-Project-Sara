public class UserService : IUserService
{
    private readonly IUserRepository urepository;

    public UserService(IUserRepository repository)
    {
        urepository = repository;
    }

    public UserResponseDto GetById(int id)
    {
        var user = urepository.GetById(id);
        if (user == null) throw new Exception("User not found");
        return user.ToDto<User, UserResponseDto>();
    }

    public List<UserResponseDto> GetAll()
    {
        var users = urepository.GetAll();
        return users.ToDtoList<User, UserResponseDto>();
    }

    public UserResponseDto Create(UserRequestDto dto)
    {
        var user = dto.ToEntity<UserRequestDto, User>();
        urepository.Add(user);
        return user.ToDto<User, UserResponseDto>();
    }

    public UserResponseDto Update(int id, UserRequestDto dto)
    {
        var user = urepository.GetById(id);
        if (user == null) throw new Exception("User not found");
        
        user.UpdateProfile(dto.FullName, dto.Phone, dto.Email);
        user.ChangeRole(dto.Role);
        user.AssignDepartment(dto.DepartmentId ?? 0);
        user.UpdateSalary(dto.SalaryAmount, dto.SalaryBonus, dto.SalaryCurrency);

        
        urepository.Update(user);
        return user.ToDto<User, UserResponseDto>();
    }

    public void Delete(int id)
    {
        urepository.Delete(id);
    }

    public UserResponseDto UpdateSalary(int userId, SalaryRequestDto dto)
{
    var user = urepository.GetById(userId);
    if (user == null) throw new Exception("User not found");
    
    user.UpdateSalary(dto.Amount, dto.Bonus, dto.Currency);
    
    urepository.Update(user);
    
    return user.ToDto<User, UserResponseDto>();
}
}