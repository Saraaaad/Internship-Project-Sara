using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly ITaskService tService;
    private readonly IAuthorizationService authzService;

    public TaskController(ITaskService taskService, IAuthorizationService authorizationService)
    {
        tService = taskService;
        authzService = authorizationService;
    }

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<TaskResponseDto> GetById(int id)
    {
        try
        {
            var task = tService.GetById(id);
            if (!authzService.CanAccessUserData(task.EmployeeId))
                return Forbid();

            return Ok(task);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("employee/{employeeId}")]
    public ActionResult<List<TaskResponseDto>> GetByEmployeeId(int employeeId)
    {
        try
        {
            var tasks = tService.GetByEmployeeId(employeeId);
            if (!authzService.CanAccessUserData(employeeId))
                return Forbid();

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("status/{status}")]
    public ActionResult<List<TaskResponseDto>> GetByStatus(Status status)
    {
        try
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
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Admin,Lead")]
    [HttpPost]
    public ActionResult<TaskResponseDto> Create([FromBody] TaskRequestDto dto)
    {
        try
        {
            var task = tService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpPatch("{id}/status")]
    public ActionResult<TaskResponseDto> UpdateStatus(int id, [FromBody] Status status)
    {
        try
        {
            var task = tService.GetById(id);
            if (!authzService.CanAccessUserData(task.EmployeeId))
                return Forbid();

            var updatedTask = tService.UpdateStatus(id, status);
            return Ok(updatedTask);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Admin,Lead")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        try
        {
            tService.Delete(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}