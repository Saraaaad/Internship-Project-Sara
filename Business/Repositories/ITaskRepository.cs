public interface ITaskRepository
{
    Tasks GetById(int id);
    List<Tasks> GetByEmployeeId(int employeeId);
    void Add(Tasks task);
    void Update(Tasks task);
    void Delete(int id);
}