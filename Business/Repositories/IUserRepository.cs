public interface IUserRepository : IRepository<User>
{
    User GetByEmail(string email);
    User GetByUsername(string username);
    List<User> GetByDepartmentId(int departmentId);
}