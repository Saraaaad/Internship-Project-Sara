using InternshipProjectSara.Data.Context;

public class TaskService : ITaskService
{
    private readonly ITaskRepository trepository;
    private readonly DatabaseContext _context;

    public TaskService(ITaskRepository repository, DatabaseContext context)
    {
        trepository = repository;
        _context = context;
    }

    public TaskResponseDto GetById(int id)
    {
        var task = trepository.GetById(id);
        if (task == null)
            throw new Exception("Task not found");
            
        return task.ToDto<Tasks, TaskResponseDto>();
    }

    public List<TaskResponseDto> GetByEmployeeId(int employeeId)
    {
        var tasks = trepository.GetByEmployeeId(employeeId);
        return tasks.ToDtoList<Tasks, TaskResponseDto>();
    }

    public List<TaskResponseDto> GetByStatus(Status status)
    {
        var tasks = trepository.GetByStatus(status);
        return tasks.ToDtoList<Tasks, TaskResponseDto>();
    }

    public TaskResponseDto Create(TaskRequestDto dto)
    {
        var task = dto.ToEntity<TaskRequestDto, Tasks>();
        trepository.Add(task);
        _context.SaveChanges();
        return task.ToDto<Tasks, TaskResponseDto>();
    }

    public TaskResponseDto UpdateStatus(int id, Status status)
    {
        var task = trepository.GetById(id);
        if (task == null)
            throw new Exception("Task not found");

        task.Status = status;
        trepository.Update(task);
        _context.SaveChanges();
        return task.ToDto<Tasks, TaskResponseDto>();
    }

    public void Delete(int id)
    {
        var task = trepository.GetById(id);
        if (task == null)
            throw new Exception("Task not found");

        trepository.Delete(id);
        _context.SaveChanges();
    }

    public TaskResponseDto Update(int id, TaskRequestDto dto)
    {
        var task = trepository.GetById(id);
        if (task == null)
            throw new Exception("Task not found");

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.EmployeeId = dto.EmployeeId;

        trepository.Update(task);
        _context.SaveChanges();
        return task.ToDto<Tasks, TaskResponseDto>();
    }
}