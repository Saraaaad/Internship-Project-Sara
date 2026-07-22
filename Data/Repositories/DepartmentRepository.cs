using InternshipProjectSara.Data.Context;
using Microsoft.EntityFrameworkCore;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(DatabaseContext context) : base(context) { }

    public override Department? GetById(int id)
    {
        return _dbSet
            .Include(d => d.Employees)
            .FirstOrDefault(d => d.Id == id);
    }

    public override List<Department> GetAll()
    {
        return _dbSet
            .Include(d => d.Employees)
            .ToList();
    }

    public Department? GetByName(string name)
    {
        return _dbSet
        .Include(d => d.Employees)
        .FirstOrDefault(d => d.Name == name);
    }
}