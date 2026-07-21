using InternshipProjectSara.Data.Context;
using Microsoft.EntityFrameworkCore;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DatabaseContext context) : base(context) { }

    public override User? GetById(int id)
    {
        return _dbSet
            .Include(u => u.Notes)
            .Include(u => u.Tasks)
            .Include(u => u.Department)
            .FirstOrDefault(u => u.Id == id);
    }
    public User? GetByEmail(string email)
    {
        return _dbSet
        .Include(u => u.Department)
        .FirstOrDefault(u => u.Email == email);
    }

    public User? GetByUsername(string username)
    {
        return _dbSet
        .Include(u => u.Department)
        .FirstOrDefault(u => u.Username == username);
    }

    public List<User> GetByDepartmentId(int departmentId)
    {
        return _dbSet.Where(u => u.DepartmentId == departmentId)
        .Include(u => u.Department).ToList();
    }
}