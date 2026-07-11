public interface ITaskService
{
    TaskResponseDto GetById(int id);
    List<TaskResponseDto> GetByEmployeeId(int employeeId);
    TaskResponseDto Create(TaskRequestDto dto);
    TaskResponseDto UpdateStatus(int id, Status status);
    List<TaskResponseDto> GetByStatus(Status status);
    List<TaskResponseDto> GetByEmployeeAndStatus(int employeeId, Status status);
    List<TaskResponseDto> GetByDeadline(DateTime deadline);
    TaskResponseDto Update(int id, TaskRequestDto dto);
    void Delete(int id);
}