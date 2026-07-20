using InternshipProjectSara.Data.Context;

public class TaskService : ITaskService
{
    private readonly ITaskRepository trepository;
    private readonly DatabaseContext _context;
    private readonly ILogger<TaskService> logger;
    private readonly ILogService logService;

    public TaskService(ITaskRepository repository, DatabaseContext context, ILogger<TaskService> logger, ILogService logService)
    {
        trepository = repository;
        _context = context;
        this.logger = logger;
        this.logService = logService;
    }

    public TaskResponseDto GetById(int id)
    {
        logger.LogInformation("Retrieving task with ID: {id}", id);
        logService.Log(LogLevel.Information, $"Retrieving task with ID: {id}");
        var task = trepository.GetById(id);
        if (task == null)
        {
            logger.LogWarning("Failed, Task with ID: {id} not found", id);
            logService.Log(LogLevel.Warning, $"Task with ID: {id} not found");
            throw new NotFoundException("Task", id);
        }
        logger.LogInformation("Task with ID: {id} retrieved successfully", id);
        logService.Log(LogLevel.Information, $"Task with ID: {id} retrieved successfully");
        return task.ToDto<Tasks, TaskResponseDto>();
    }

    public List<TaskResponseDto> GetByEmployeeId(int employeeId)
    {
        logger.LogInformation("Retrieving tasks for employee with ID: {employeeId}", employeeId);
        logService.Log(LogLevel.Information, $"Retrieving tasks for employee with ID: {employeeId}");
        var employee = _context.Users.Find(employeeId);
        if (employee == null)
        {
            logger.LogWarning("Failed, Employee with ID: {employeeId} not found", employeeId);
            logService.Log(LogLevel.Warning, $"Employee with ID: {employeeId} not found");
            throw new NotFoundException("Employee", employeeId);
        }

        var tasks = trepository.GetByEmployeeId(employeeId);
        if (tasks == null || tasks.Count == 0)
        {
            logger.LogInformation("No tasks found for employee with ID: {employeeId}", employeeId);
            logService.Log(LogLevel.Information, $"No tasks found for employee with ID: {employeeId}");
        }

        logger.LogInformation("tasks retrieved successfully for employee with ID: {employeeId}", employeeId);
        logService.Log(LogLevel.Information, $"tasks retrieved successfully for employee with ID: {employeeId}");

        return tasks.ToDtoList<Tasks, TaskResponseDto>();
    }

    public List<TaskResponseDto> GetByStatus(Status status)
    {
        logger.LogInformation("Retrieving tasks with status: {status}", status);
        logService.Log(LogLevel.Information, $"Retrieving tasks with status: {status}");
        if (!Enum.IsDefined(typeof(Status), status))
        {
            logger.LogWarning("Invalid status value: {status}. Valid values are: {validValues}", status, string.Join(", ", Enum.GetNames(typeof(Status))));
            logService.Log(LogLevel.Warning, $"Invalid status value: {status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");
            throw new ValidationException($"Invalid status value: {status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");
        }

        var tasks = trepository.GetByStatus(status);
        logger.LogInformation("tasks retrieved successfully with status: {status}", status);
        logService.Log(LogLevel.Information, $"tasks retrieved successfully with status: {status}");
        return tasks.ToDtoList<Tasks, TaskResponseDto>();
    }

    public List<TaskResponseDto> GetByEmployeeAndStatus(int employeeId, Status status)
    {
        logger.LogInformation("Retrieving tasks for employee with ID: {employeeId} and status: {status}", employeeId, status);
        logService.Log(LogLevel.Information, $"Retrieving tasks for employee with ID: {employeeId} and status: {status}");
        if (!Enum.IsDefined(typeof(Status), status))
        {
            logger.LogWarning("Invalid status value: {status}. Valid values are: {validValues}", status, string.Join(", ", Enum.GetNames(typeof(Status))));
            logService.Log(LogLevel.Warning, $"Invalid status value: {status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");
            throw new ValidationException($"Invalid status value: {status}");
        }
        var employee = _context.Users.Find(employeeId);
        if (employee == null)
        {
            logger.LogWarning("Failed, Employee with ID: {employeeId} not found", employeeId);
            logService.Log(LogLevel.Warning, $"Employee with ID: {employeeId} not found");
            throw new NotFoundException("Employee", employeeId);
        }
        var tasks = trepository.GetByEmployeeAndStatus(employeeId, status);
        if (tasks == null || tasks.Count == 0)
        {
            logService.Log(LogLevel.Information, $"No tasks found for employee with ID: {employeeId} and status: {status}");
            logger.LogInformation("No tasks found for employee with ID: {employeeId} and status: {status}", employeeId, status);
        }
        logService.Log(LogLevel.Information, $"tasks retrieved successfully for employee with ID: {employeeId} and status: {status}");
        logger.LogInformation("tasks retrieved successfully for employee with ID: {employeeId} and status: {status}", employeeId, status);
        return tasks.ToDtoList<Tasks, TaskResponseDto>();
    }

    public TaskResponseDto Create(TaskRequestDto dto)
    {
        logger.LogInformation("Creating a new task");
        logService.Log(LogLevel.Information, $"Creating a new task");
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var task = dto.ToEntity<TaskRequestDto, Tasks>();
        var employee = _context.Users.Find(task.EmployeeId);
        if (employee == null)
        {
            logService.Log(LogLevel.Warning, $"Employee with ID: {task.EmployeeId} not found");
            logger.LogWarning("Failed, Employee with ID: {employeeId} not found", task.EmployeeId);
            throw new NotFoundException("Employee", task.EmployeeId);
        }
        if (!Enum.IsDefined(typeof(Status), task.Status))
        {
            logger.LogWarning("Invalid status value: {status}. Valid values are: {validValues}", task.Status, string.Join(", ", Enum.GetNames(typeof(Status))));
            logService.Log(LogLevel.Warning, $"Invalid status value: {task.Status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");
            throw new ValidationException($"Invalid status value: {task.Status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");
        }
        if (task.Deadline < DateTime.Now)
        {
            logger.LogWarning("Invalid deadline value: {deadline}. Deadline cannot be in the past", task.Deadline);
            logService.Log(LogLevel.Warning, $"Deadline cannot be in the past");
            throw new ValidationException("Deadline cannot be in the past");
        }
        trepository.Add(task);
        _context.SaveChanges();
        logger.LogInformation("Task created successfully with ID: {id}", task.Id);
        logService.Log(LogLevel.Information, $"Task created successfully with ID: {task.Id}");
        return task.ToDto<Tasks, TaskResponseDto>();
    }

    public TaskResponseDto UpdateStatus(int id, Status status)
    {
        logger.LogInformation("Updating status for task with ID: {id}", id);
        logService.Log(LogLevel.Information, $"Updating status for task with ID: {id}");
        var task = trepository.GetById(id);
        if (task == null)
        {
            logger.LogWarning("Failed, Task with ID: {id} not found", id);
            logService.Log(LogLevel.Warning, $"Task with ID: {id} not found");
            throw new NotFoundException("Task", id);
        }
        if (!Enum.IsDefined(typeof(Status), status))
        {
            logger.LogWarning("Invalid status value: {status}. Valid values are: {validValues}", status, string.Join(", ", Enum.GetNames(typeof(Status))));
            logService.Log(LogLevel.Warning, $"Invalid status value: {status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");
            throw new ValidationException($"Invalid status value: {status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");
        }
        task.Status = status;
        trepository.Update(task);
        _context.SaveChanges();
        logger.LogInformation("Status for task with ID: {id} updated successfully to {status}", id, status);
        logService.Log(LogLevel.Information, $"Status for task with ID: {id} updated successfully to {status}");
        return task.ToDto<Tasks, TaskResponseDto>();
    }

    public void Delete(int id)
    {
        logger.LogInformation("Deleting task with ID: {id}", id);
        logService.Log(LogLevel.Information, $"Deleting task with ID: {id}");
        var task = trepository.GetById(id);
        if (task == null)
        {
            logger.LogWarning("Failed, Task with ID: {id} not found", id);
            logService.Log(LogLevel.Warning, $"Task with ID: {id} not found");
            throw new NotFoundException("Task", id);
        }
        trepository.Delete(id);
        logService.Log(LogLevel.Information, $"Task with ID: {id} deleted successfully");
        _context.SaveChanges();
        logger.LogInformation("Task with ID: {id} deleted successfully", id);
    }

    public TaskResponseDto Update(int id, TaskRequestDto dto)
    {
        logger.LogInformation("Updating task with ID: {id}", id);
        logService.Log(LogLevel.Information, $"Updating task with ID: {id}");
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var task = trepository.GetById(id);
        if (task == null)
        {
            logger.LogWarning("Failed, Task with ID: {id} not found", id);
            logService.Log(LogLevel.Warning, $"Task with ID: {id} not found");
            throw new NotFoundException("Task", id);
        }
        if (!Enum.IsDefined(typeof(Status), dto.Status))
        {
            logger.LogWarning("Invalid status value: {status}. Valid values are: {validValues}", dto.Status, string.Join(", ", Enum.GetNames(typeof(Status))));
            logService.Log(LogLevel.Warning, $"Invalid status value: {dto.Status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");
            throw new ValidationException($"Invalid status value: {dto.Status}. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Status)))}");
        }
        if (dto.Deadline < DateTime.Now)
        {
            logger.LogWarning("Invalid deadline value: {deadline}. Deadline cannot be in the past", dto.Deadline);
            logService.Log(LogLevel.Warning, $"Deadline cannot be in the past");
            throw new ValidationException("Deadline cannot be in the past");
        }
        task.Title = dto.Title;
        task.Description = dto.Description;
        task.EmployeeId = dto.EmployeeId;

        trepository.Update(task);
        _context.SaveChanges();
        logService.Log(LogLevel.Information, $"Task with ID: {id} updated successfully");
        logger.LogInformation("Task with ID: {id} updated successfully", id);
        return task.ToDto<Tasks, TaskResponseDto>();
    }
    public List<TaskResponseDto> GetByDeadline(DateTime deadline)
    {
        logger.LogInformation("Retrieving tasks with deadline: {deadline}", deadline);
        logService.Log(LogLevel.Information, $"Retrieving tasks with deadline: {deadline}");
        var tasks = trepository.GetByDeadline(deadline);
        if (tasks == null || tasks.Count == 0)
        {
            logService.Log(LogLevel.Information, $"No tasks found with deadline: {deadline}");
            logger.LogInformation("No tasks found with deadline: {deadline}", deadline);
        }
        logService.Log(LogLevel.Information, $"tasks retrieved successfully with deadline: {deadline}");
        logger.LogInformation("tasks retrieved successfully with deadline: {deadline}", deadline);
        return tasks.ToDtoList<Tasks, TaskResponseDto>();
    }
}