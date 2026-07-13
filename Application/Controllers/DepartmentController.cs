using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/departments")]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentService dService;
    private readonly IAuthorizationService authzService;

    public DepartmentController(IDepartmentService departmentService, IAuthorizationService authorizationService)
    {
        dService = departmentService;
        authzService = authorizationService;
    }

    [Authorize]
    [HttpGet]
    public ActionResult<List<DepartmentResponseDto>> GetAll()
    {
        try
        {
            var departments = dService.GetAll();
            return Ok(departments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<DepartmentResponseDto> GetById(int id)
    {
        var department = dService.GetById(id);
        return Ok(department);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public ActionResult<DepartmentResponseDto> Create([FromBody] DepartmentRequestDto dto)
    {
        try
        {
            var department = dService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}")]
    public ActionResult<DepartmentResponseDto> Update(int id, [FromBody] DepartmentRequestDto dto)
    {
        try
        {
            var department = dService.Update(id, dto);
            return Ok(department);

        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        try
        {
            dService.Delete(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize]
    [HttpGet("name/{name}")]
    public ActionResult<DepartmentResponseDto> GetByName(string name)
    {
        try
        {
            var department = dService.GetByName(name);
            return Ok(department);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}