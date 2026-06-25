using InternshipProjectSara.Data.Context;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DatabaseContext context) : base(context) { }

    public User? GetByEmail(string email)
    {
        return _dbSet.FirstOrDefault(u => u.Email == email);
    }

    public User? GetByUsername(string username)
    {
        return _dbSet.FirstOrDefault(u => u.Username == username);
    }

    public List<User> GetByDepartmentId(int departmentId)
    {
        return _dbSet.Where(u => u.DepartmentId == departmentId).ToList();
    }

}