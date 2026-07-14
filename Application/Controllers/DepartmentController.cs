using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/departments")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService dService;

    public DepartmentController(IDepartmentService departmentService)
    {
        dService = departmentService;
    }

    [Authorize]
    [HttpGet]
    public ActionResult<List<DepartmentResponseDto>> GetAll()
    {
        var departments = dService.GetAll();
        return Ok(departments);
    }

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<DepartmentResponseDto> GetById(int id)
    {
        try
        {
            var department = dService.GetById(id);
            return Ok(department);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public ActionResult<DepartmentResponseDto> Create([FromBody] DepartmentRequestDto dto)
    {
        var department = dService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}")]
    public ActionResult<DepartmentResponseDto> Update(int id, [FromBody] DepartmentRequestDto dto)
    {
        var department = dService.Update(id, dto);
        return Ok(department);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        dService.Delete(id);
        return NoContent();
    }

    [Authorize]
    [HttpGet("name/{name}")]
    public ActionResult<DepartmentResponseDto> GetByName(string name)
    {
        var department = dService.GetByName(name);
        return Ok(department);
    }
}