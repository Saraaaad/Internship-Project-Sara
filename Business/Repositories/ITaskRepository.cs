public interface ITaskRepository : IRepository<Tasks>
{
    List<Tasks> GetByEmployeeId(int employeeId);
    List<Tasks> GetByStatus(Status status);
    List<Tasks> GetByDeadline(DateTime deadline);
}