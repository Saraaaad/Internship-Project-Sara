using InternshipProjectSara.Data.Context;

public class TaskService : ITaskService
{
    private readonly ITaskRepository trepository;
    private readonly DatabaseContext _context;
    private readonly ILogger<TaskService> logger;

    public TaskService(ITaskRepository repository, DatabaseContext context, ILogger<TaskService> logger)
    {
        trepository = repository;
        _context = context;
        this.logger = logger;
    }

    public TaskResponseDto GetById(int id)
    {
        logger.LogInformation("Retrieving task with ID: {id}", id);
        var task = trepository.GetById(id);
        if (task == null)
        {
            logger.LogWarning("Failed, Task with ID: {id} not found", id);
            throw new NotFoundException("Task", id);
        }
        logger.LogInformation("Task with ID: {id} retrieved successfully", id);
        return task.ToDto<Tasks, TaskResponseDto>();
    }

    public List<TaskResponseDto> GetByEmployeeId(int employeeId)
    {
        logger.LogInformation("Retrieving tasks for employee with ID: {employeeId}", employeeId);
        var employee = _context.Users.Find(employeeId);
        if (employee == null)
        {
            logger.LogWarning("Failed, Employee with ID: {employeeId} not found", employeeId);
            throw new NotFoundException("Employee", employeeId);
        }

        var tasks = trepository.GetByEmployeeId(employeeId);
        if (tasks == null || tasks.Count == 0)
            logger.LogInformation("No tasks found for employee with ID: {employeeId}", employeeId);

        logger.LogInformation("tasks retrieved successfully for employee with ID: {employeeId}", employeeId);
        return tasks.ToDtoList<Tasks, TaskResponseDto>();
    }

    public List<TaskResponseDto> GetByStatus(Status status)
    {
        logger.LogInformation("Retrieving tasks with status: {status}", status);
        if (!Enum.IsDefined(typeof(Status), status))
        {
            logger.LogWarning("Invalid status value: {status}. Valid values are: {validValues}", status, string.Join(", ", Enum.GetNames(typeof(Status))));
            throw new ValidationException($"Invalid status value: {status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");
        }

        var tasks = trepository.GetByStatus(status);
        logger.LogInformation("tasks retrieved successfully with status: {status}", status);
        return tasks.ToDtoList<Tasks, TaskResponseDto>();
    }

    public List<TaskResponseDto> GetByEmployeeAndStatus(int employeeId, Status status)
    {
        logger.LogInformation("Retrieving tasks for employee with ID: {employeeId} and status: {status}", employeeId, status);
        if (!Enum.IsDefined(typeof(Status), status))
        {
            logger.LogWarning("Invalid status value: {status}. Valid values are: {validValues}", status, string.Join(", ", Enum.GetNames(typeof(Status))));
            throw new ValidationException($"Invalid status value: {status}");
        }
        var employee = _context.Users.Find(employeeId);
        if (employee == null)
        {
            logger.LogWarning("Failed, Employee with ID: {employeeId} not found", employeeId);
            throw new NotFoundException("Employee", employeeId);
        }
        var tasks = trepository.GetByEmployeeAndStatus(employeeId, status);
        if (tasks == null || tasks.Count == 0)
            logger.LogInformation("No tasks found for employee with ID: {employeeId} and status: {status}", employeeId, status);

        logger.LogInformation("tasks retrieved successfully for employee with ID: {employeeId} and status: {status}", employeeId, status);
        return tasks.ToDtoList<Tasks, TaskResponseDto>();
    }

    public TaskResponseDto Create(TaskRequestDto dto)
    {
        logger.LogInformation("Creating a new task");
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var task = dto.ToEntity<TaskRequestDto, Tasks>();
        var employee = _context.Users.Find(task.EmployeeId);
        if (employee == null)
        {
            logger.LogWarning("Failed, Employee with ID: {employeeId} not found", task.EmployeeId);
            throw new NotFoundException("Employee", task.EmployeeId);
        }
        if (!Enum.IsDefined(typeof(Status), task.Status))
        {
            logger.LogWarning("Invalid status value: {status}. Valid values are: {validValues}", task.Status, string.Join(", ", Enum.GetNames(typeof(Status))));
            throw new ValidationException($"Invalid status value: {task.Status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");
        }
        if (task.Deadline < DateTime.Now)
        {
            logger.LogWarning("Invalid deadline value: {deadline}. Deadline cannot be in the past", task.Deadline);
            throw new ValidationException("Deadline cannot be in the past");
        }
        trepository.Add(task);
        _context.SaveChanges();
        logger.LogInformation("Task created successfully with ID: {id}", task.Id);
        return task.ToDto<Tasks, TaskResponseDto>();
    }

    public TaskResponseDto UpdateStatus(int id, Status status)
    {
        logger.LogInformation("Updating status for task with ID: {id}", id);
        var task = trepository.GetById(id);
        if (task == null)
        {
            logger.LogWarning("Failed, Task with ID: {id} not found", id);
            throw new NotFoundException("Task", id);
        }
        if (!Enum.IsDefined(typeof(Status), status))
        {
            logger.LogWarning("Invalid status value: {status}. Valid values are: {validValues}", status, string.Join(", ", Enum.GetNames(typeof(Status))));
            throw new ValidationException($"Invalid status value: {status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");
        }
        task.Status = status;
        trepository.Update(task);
        _context.SaveChanges();
        logger.LogInformation("Status for task with ID: {id} updated successfully to {status}", id, status);
        return task.ToDto<Tasks, TaskResponseDto>();
    }

    public void Delete(int id)
    {
        logger.LogInformation("Deleting task with ID: {id}", id);
        var task = trepository.GetById(id);
        if (task == null)
        {
            logger.LogWarning("Failed, Task with ID: {id} not found", id);
            throw new NotFoundException("Task", id);
        }
        trepository.Delete(id);
        _context.SaveChanges();
        logger.LogInformation("Task with ID: {id} deleted successfully", id);
    }

    public TaskResponseDto Update(int id, TaskRequestDto dto)
    {
        logger.LogInformation("Updating task with ID: {id}", id);
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var task = trepository.GetById(id);
        if (task == null)
        {
            logger.LogWarning("Failed, Task with ID: {id} not found", id);
            throw new NotFoundException("Task", id);
        }
        if (!Enum.IsDefined(typeof(Status), dto.Status))
        {
            logger.LogWarning("Invalid status value: {status}. Valid values are: {validValues}", dto.Status, string.Join(", ", Enum.GetNames(typeof(Status))));
            throw new ValidationException($"Invalid status value: {dto.Status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");
        }
        if (dto.Deadline < DateTime.Now)
        {
            logger.LogWarning("Invalid deadline value: {deadline}. Deadline cannot be in the past", dto.Deadline);
            throw new ValidationException("Deadline cannot be in the past");
        }
        task.Title = dto.Title;
        task.Description = dto.Description;
        task.EmployeeId = dto.EmployeeId;

        trepository.Update(task);
        _context.SaveChanges();
        logger.LogInformation("Task with ID: {id} updated successfully", id);
        return task.ToDto<Tasks, TaskResponseDto>();
    }
    public List<TaskResponseDto> GetByDeadline(DateTime deadline)
    {
        logger.LogInformation("Retrieving tasks with deadline: {deadline}", deadline);
        var tasks = trepository.GetByDeadline(deadline);
        if (tasks == null || tasks.Count == 0)
            logger.LogInformation("No tasks found with deadline: {deadline}", deadline);

        logger.LogInformation("tasks retrieved successfully with deadline: {deadline}", deadline);
        return tasks.ToDtoList<Tasks, TaskResponseDto>();
    }
}