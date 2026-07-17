using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/logs")]
[Authorize(Roles = "Admin")]
public class LogController : ControllerBase
{
    private readonly ILogService logService;

    public LogController(ILogService logService)
    {
        this.logService = logService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var logs = logService.GetAll();
        return Ok(logs);
    }

    [HttpGet("errors")]
    public IActionResult GetAllErrors()
    {
        var logs = logService.GetAllErrors();
        return Ok(logs);
    }

    [HttpGet("date/{date}")]
    public IActionResult GetByDate(DateTime date)
    {
        var logs = logService.GetByDate(date);
        return Ok(logs);
    }

    [HttpGet("level/{level}")]
    public IActionResult GetByLevel(LogLevel level)
    {
        var logs = logService.GetByLevel(level);
        return Ok(logs);
    }

    [HttpGet("date-level/{date}/{level}")]
    public IActionResult GetByDateAndLevel(DateTime date, LogLevel level)
    {
        var logs = logService.GetByDateAndLevel(date, level);
        return Ok(logs);
    }

    [HttpDelete("clear/{lastDays}")]
    public IActionResult Clear(int lastDays)
    {
        logService.Clear(lastDays);
        return NoContent();
    }

    [HttpGet("date-range/{from}/{to}")]
    public IActionResult GetByDateRange(DateTime from, DateTime to)
    {
        var logs = logService.GetByDateRange(from, to);
        return Ok(logs);
    }
}