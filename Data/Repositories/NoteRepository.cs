
using InternshipProjectSara.Data.Context;

public class NoteRepository : Repository<Note>, INoteRepository
{
    public NoteRepository(DatabaseContext context) : base(context) { }

    public List<Note> GetByEmployeeId(int employeeId)
    {
        return _dbSet.Where(n => n.EmployeeId == employeeId).ToList();
    }
}