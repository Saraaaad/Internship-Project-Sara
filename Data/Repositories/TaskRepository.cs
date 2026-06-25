using InternshipProjectSara.Data.Context;

public class TaskRepository : Repository<Tasks>, ITaskRepository
{
    public TaskRepository(DatabaseContext context) : base(context) { }

    public List<Tasks> GetByEmployeeId(int employeeId)
    {
        return _dbSet.Where(t => t.EmployeeId == employeeId).ToList();
    }

    public List<Tasks> GetByStatus(Status status)
    {
        return _dbSet.Where(t => t.Status == status).ToList();
    }
}