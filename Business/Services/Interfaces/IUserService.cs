public interface IUserService
{
    UserResponseDto GetById(int id);
    List<UserResponseDto> GetAll();
    UserResponseDto Create(UserRequestDto dto);
    UserResponseDto Update(int id, UserRequestDto dto);
    void Delete(int id);
    UserResponseDto UpdateSalary(int userId, SalaryRequestDto dto); 

}