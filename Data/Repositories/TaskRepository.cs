using InternshipProjectSara.Data.Context;
using Microsoft.EntityFrameworkCore;

public class TaskRepository : Repository<Tasks>, ITaskRepository
{
    public TaskRepository(DatabaseContext context) : base(context) { }

    public override List<Tasks> GetAll(){
        return _dbSet.Include(t => t.Employee).ToList();
    }
    public override Tasks GetById(int id){
        return _dbSet.Include(t => t.Employee).FirstOrDefault(t => t.Id == id);
    }

    public List<Tasks> GetByEmployeeId(int employeeId)
    {
        return _dbSet.Where(t => t.EmployeeId == employeeId).ToList();
    }

    public List<Tasks> GetByStatus(Status status)
    {
        return _dbSet.Where(t => t.Status == status).ToList();
    }
    public List<Tasks> GetByDeadline(DateTime deadline)
    {
        return _dbSet.Where(t => t.Deadline == deadline).ToList();
    }
    public List<Tasks> GetByEmployeeAndStatus(int employeeId, Status status)
    {
        return _dbSet.Where(t => t.EmployeeId == employeeId && t.Status == status).ToList();
    }
}