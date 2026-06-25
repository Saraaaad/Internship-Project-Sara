public interface INoteRepository : IRepository<Note>
{    List<Note> GetByEmployeeId(int employeeId);
}