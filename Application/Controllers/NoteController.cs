using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/notes")]
public class NoteController : ControllerBase
{
    private readonly INoteService nService;

    public NoteController(INoteService noteService)
    {
        nService = noteService;
    }

    [HttpGet("{id}")]
    public ActionResult<NoteResponseDto> GetById(int id)
    {
        try
        {
            var note = nService.GetById(id);
            return Ok(note);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("employee/{employeeId}")]
    public ActionResult<List<NoteResponseDto>> GetByEmployeeId(int employeeId)
    {
        var notes = nService.GetByEmployeeId(employeeId);
        return Ok(notes);
    }

    [HttpPost]
    public ActionResult<NoteResponseDto> Create([FromBody] NoteRequestDto dto)
    {
        var note = nService.Create(dto);
        if (note == null)
            return BadRequest("Failed to create note.");

        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }

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
            return NotFound(ex.Message);
        }
    }

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
            return NotFound(ex.Message);
        }
    }
}