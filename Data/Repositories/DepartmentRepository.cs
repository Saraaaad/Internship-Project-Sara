using InternshipProjectSara.Data.Context;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(DatabaseContext context) : base(context) { }

    public Department? GetByName(string name)
    {
        return _dbSet.FirstOrDefault(d => d.Name == name);
    }
}