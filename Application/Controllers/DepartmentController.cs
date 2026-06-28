
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

    [HttpGet]
    public ActionResult<List<DepartmentResponseDto>> GetAll()
    {

        var departments = dService.GetAll();
        return Ok(departments);
    }

    [HttpGet("{id}")]
    public ActionResult<DepartmentResponseDto> GetById(int id)
    {

        var department = dService.GetById(id);
        return Ok(department);
    }

    [HttpPost]
    public ActionResult<DepartmentResponseDto> Create([FromBody] DepartmentRequestDto dto)
    {

        var department = dService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
    }

    [HttpPut("{id}")]
    public ActionResult<DepartmentResponseDto> Update(int id, [FromBody] DepartmentRequestDto dto)
    {

        var department = dService.Update(id, dto);
        return Ok(department);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {

        dService.Delete(id);
        return NoContent();
    }

    [HttpGet("name/{name}")]
    public ActionResult<DepartmentResponseDto> GetByName(string name)
    {

        var department = dService.GetByName(name);
        return Ok(department);
    }
}