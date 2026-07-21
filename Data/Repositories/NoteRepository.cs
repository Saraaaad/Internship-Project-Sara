
using InternshipProjectSara.Data.Context;
using Microsoft.EntityFrameworkCore;

public class NoteRepository : Repository<Note>, INoteRepository
{
    public NoteRepository(DatabaseContext context) : base(context) { }

    public override Note GetById(int id)
    {
        return _dbSet
        .Include(n => n.Employee)
        .FirstOrDefault(n => n.Id == id);
    }
    public override List<Note> GetAll()
    {
        return _dbSet
        .Include(n => n.Employee)
        .ToList();
    }

    public List<Note> GetByEmployeeId(int employeeId)
    {
        return _dbSet
        .Include(n => n.Employee)
        .Where(n => n.EmployeeId == employeeId).ToList();
    }
}