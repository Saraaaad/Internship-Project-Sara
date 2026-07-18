using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly ITaskService tService;
    private readonly IAuthorizationService authzService;
    private readonly ILogger<TaskController> logger;
    private readonly ILogService logService;

    public TaskController(ITaskService taskService, IAuthorizationService authorizationService, ILogger<TaskController> logger, ILogService logService)
    {
        tService = taskService;
        authzService = authorizationService;
        this.logger = logger;
        this.logService = logService;
    }

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<TaskResponseDto> GetById(int id)
    {
        var task = tService.GetById(id);
        if (!authzService.CanAccessUserData(task.EmployeeId))
        {
            logService.Log(LogLevel.Warning, $"User with id: {authzService.GetCurrentUserId()} tried to access another user's task with id : {task.EmployeeId}");
            logger.LogWarning("User with id: {current} tried to access another user's task with id : {id}", authzService.GetCurrentUserId(), task.EmployeeId);
            return Forbid();
        }
        return Ok(task);
    }

    [Authorize]
    [HttpGet("employee/{employeeId}")]
    public ActionResult<List<TaskResponseDto>> GetByEmployeeId(int employeeId)
    {
        if (!authzService.CanAccessUserData(employeeId))
        {
            logService.Log(LogLevel.Warning, $"User with id: {authzService.GetCurrentUserId()} tried to access data for user with id : {employeeId}");
            logger.LogWarning("User with id: {current} tried to access data for user with id : {id}", authzService.GetCurrentUserId(), employeeId);
            return Forbid();
        }
        var tasks = tService.GetByEmployeeId(employeeId);
        return Ok(tasks);
    }

    [Authorize]
    [HttpGet("status/{status}")]
    public ActionResult<List<TaskResponseDto>> GetByStatus(Status status)
    {
        List<TaskResponseDto> tasks;
        if (authzService.IsAdminOrHR())
        {
            tasks = tService.GetByStatus(status);
        }
        else
        {
            var currentId = authzService.GetCurrentUserId();
            tasks = tService.GetByEmployeeAndStatus(currentId, status);
        }
        return Ok(tasks);
    }

    [Authorize(Roles = "Admin,Lead")]
    [HttpPost]
    public ActionResult<TaskResponseDto> Create([FromBody] TaskRequestDto dto)
    {
        var task = tService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [Authorize]
    [HttpPatch("{id}/status")]
    public ActionResult<TaskResponseDto> UpdateStatus(int id, [FromBody] Status status)
    {
        var task = tService.GetById(id);
        if (!authzService.CanAccessUserData(task.EmployeeId))
        {
            logService.Log(LogLevel.Warning, $"User with id: {authzService.GetCurrentUserId()} tried to access data for user with id : {task.EmployeeId}");
            logger.LogWarning("User with id: {current} tried to access data for user with id : {id}", authzService.GetCurrentUserId(), task.EmployeeId);
            return Forbid();
        }
        var updatedTask = tService.UpdateStatus(id, status);
        return Ok(updatedTask);
    }

    [Authorize(Roles = "Admin,Lead")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        tService.Delete(id);
        return NoContent();
    }
}