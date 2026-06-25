public interface INoteRepository
{
    Note GetById(int id);
    List<Note> GetByEmployeeId(int employeeId);
    void Add(Note note);
    void Update(Note note);
    void Delete(int id);
}