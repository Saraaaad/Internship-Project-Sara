using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/notes")]
public class NoteController : ControllerBase
{
    private readonly INoteService nService;
    private readonly IAuthorizationService authzService;

    public NoteController(INoteService noteService, IAuthorizationService authorizationService)
    {
        nService = noteService;
        authzService = authorizationService;
    }

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<NoteResponseDto> GetById(int id)
    {
        try
        {
            var note = nService.GetById(id);
            if (!authzService.CanAccessUserData(note.EmployeeId))
                return Forbid();

            return Ok(note);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("employee/{employeeId}")]
    public ActionResult<List<NoteResponseDto>> GetByEmployeeId(int employeeId)
    {
        try
        {
            var notes = nService.GetByEmployeeId(employeeId);
            if (!authzService.CanAccessUserData(employeeId))
                return Forbid();

            return Ok(notes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Admin,Lead")]
    [HttpPost]
    public ActionResult<NoteResponseDto> Create([FromBody] NoteRequestDto dto)
    {
        try
        {
            var note = nService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Admin,Lead")]
    [HttpPut("{id}")]
    public ActionResult<NoteResponseDto> Update(int id, [FromBody] NoteRequestDto dto)
    {
        try
        {
            var note = nService.Update(id, dto);
            return Ok(note);
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
            nService.Delete(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}