public interface IDepartmentRepository : IRepository<Department>
{
    Department GetByName(string name);
}