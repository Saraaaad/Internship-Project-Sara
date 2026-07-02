using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly ITaskService tService;

    public TaskController(ITaskService taskService)
    {
        tService = taskService;
    }

    [HttpGet("{id}")]
    public ActionResult<TaskResponseDto> GetById(int id)
    {
        try
        {
            var task = tService.GetById(id);
            return Ok(task);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("employee/{employeeId}")]
    public ActionResult<List<TaskResponseDto>> GetByEmployeeId(int employeeId)
    {
        try
        {
            var tasks = tService.GetByEmployeeId(employeeId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("status/{status}")]
    public ActionResult<List<TaskResponseDto>> GetByStatus(Status status)
    {
        try
        {
            var tasks = tService.GetByStatus(status);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

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

    [HttpPatch("{id}/status")]
    public ActionResult<TaskResponseDto> UpdateStatus(int id, [FromBody] Status status)
    {
        try
        {
            var task = tService.UpdateStatus(id, status);
            return Ok(task);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

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