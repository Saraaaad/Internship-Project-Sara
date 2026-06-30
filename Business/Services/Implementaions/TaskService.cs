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
            throw new NotFoundException("Task", id);

        return task.ToDto<Tasks, TaskResponseDto>();
    }

    public List<TaskResponseDto> GetByEmployeeId(int employeeId)
    {
        var employee = _context.Users.Find(employeeId);
        if (employee == null)
            throw new NotFoundException("Employee", employeeId);

        var tasks = trepository.GetByEmployeeId(employeeId);
        if (tasks == null || tasks.Count == 0)
            throw new NotFoundException($"No tasks found for employee with ID {employeeId}");

        return tasks.ToDtoList<Tasks, TaskResponseDto>();
    }

    public List<TaskResponseDto> GetByStatus(Status status)
    {
        if (!Enum.IsDefined(typeof(Status), status))
            throw new ValidationException($"Invalid status value: {status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");

        var tasks = trepository.GetByStatus(status);
        if (tasks == null || tasks.Count == 0)
            throw new NotFoundException($"No tasks found with status {status}");
            
        return tasks.ToDtoList<Tasks, TaskResponseDto>();
    }

    public TaskResponseDto Create(TaskRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var task = dto.ToEntity<TaskRequestDto, Tasks>();
        var employee = _context.Users.Find(task.EmployeeId);
        if (employee == null)
            throw new NotFoundException("Employee", task.EmployeeId);

        if (!Enum.IsDefined(typeof(Status), task.Status))
            throw new ValidationException($"Invalid status value: {task.Status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");

        if (task.Deadline < DateTime.Now)
            throw new ValidationException("Deadline cannot be in the past");

        trepository.Add(task);
        _context.SaveChanges();
        return task.ToDto<Tasks, TaskResponseDto>();
    }

    public TaskResponseDto UpdateStatus(int id, Status status)
    {
        var task = trepository.GetById(id);
        if (task == null)
            throw new NotFoundException("Task", id);
        if (!Enum.IsDefined(typeof(Status), status))
            throw new ValidationException($"Invalid status value: {status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");

        task.Status = status;
        trepository.Update(task);
        _context.SaveChanges();
        return task.ToDto<Tasks, TaskResponseDto>();
    }

    public void Delete(int id)
    {
        var task = trepository.GetById(id);
        if (task == null)
            throw new NotFoundException("Task", id);

        trepository.Delete(id);
        _context.SaveChanges();
    }

    public TaskResponseDto Update(int id, TaskRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var task = trepository.GetById(id);
        if (task == null)
            throw new NotFoundException("Task", id);

        if (!Enum.IsDefined(typeof(Status), dto.Status))
            throw new ValidationException($"Invalid status value: {dto.Status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");

        if (dto.Deadline < DateTime.Now)
            throw new ValidationException("Deadline cannot be in the past");

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.EmployeeId = dto.EmployeeId;

        trepository.Update(task);
        _context.SaveChanges();
        return task.ToDto<Tasks, TaskResponseDto>();
    }
    public List<TaskResponseDto> GetByDeadline(DateTime deadline)
    {
        var tasks = trepository.GetByDeadline(deadline);
        if (tasks == null || tasks.Count == 0)
            throw new NotFoundException($"No tasks found with deadline {deadline}");
        return tasks.ToDtoList<Tasks, TaskResponseDto>();
    }
}