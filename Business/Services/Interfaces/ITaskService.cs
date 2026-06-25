public interface ITaskService
{
    TaskResponseDto GetById(int id);
    List<TaskResponseDto> GetByEmployeeId(int employeeId);
    TaskResponseDto Create(TaskRequestDto dto);
    TaskResponseDto UpdateStatus(int id, Status status);
    TaskResponseDto Update(int id, TaskRequestDto dto);
    void Delete(int id);
}