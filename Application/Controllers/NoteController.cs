using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/notes")]
public class NoteController : ControllerBase
{
    private readonly INoteService nService;
    private readonly IAuthorizationService authzService;
    private readonly ILogger<NoteController> logger;

    public NoteController(INoteService noteService, IAuthorizationService authorizationService, ILogger<NoteController> logger)
    {
        nService = noteService;
        authzService = authorizationService;
        this.logger = logger;
    }

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<NoteResponseDto> GetById(int id)
    {
        var note = nService.GetById(id);
        if (!authzService.CanAccessUserData(note.EmployeeId))
        {
            logger.LogWarning("User with id: {current} tried to access data for user with id : {id}", authzService.GetCurrentUserId(), note.EmployeeId);
            return Forbid();
        }
        return Ok(note);
    }

    [Authorize]
    [HttpGet("employee/{employeeId}")]
    public ActionResult<List<NoteResponseDto>> GetByEmployeeId(int employeeId)
    {
        if (!authzService.CanAccessUserData(employeeId))
        {
            logger.LogWarning("User with id: {current} tried to access data for user with id : {id}", authzService.GetCurrentUserId(), employeeId);
            return Forbid();
        }
        var notes = nService.GetByEmployeeId(employeeId);
        return Ok(notes);
    }

    [Authorize(Roles = "Admin,Lead")]
    [HttpPost]
    public ActionResult<NoteResponseDto> Create([FromBody] NoteRequestDto dto)
    {
        var note = nService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }

    [Authorize(Roles = "Admin,Lead")]
    [HttpPut("{id}")]
    public ActionResult<NoteResponseDto> Update(int id, [FromBody] NoteRequestDto dto)
    {
        var note = nService.Update(id, dto);
        return Ok(note);
    }

    [Authorize(Roles = "Admin,Lead")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        nService.Delete(id);
        return NoContent();
    }
}