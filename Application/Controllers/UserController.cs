using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService uService;

    public UserController(IUserService userService)
    {
        uService = userService;
    }

    [HttpGet]
    public ActionResult<List<UserResponseDto>> GetAll()
    {
        try
        {
            var users = uService.GetAll();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<UserResponseDto> GetById(int id)
    {
        try
        {
            var user = uService.GetById(id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public ActionResult<UserResponseDto> Create([FromBody] UserRequestDto dto)
    {
        try
        {
            var user = uService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}")]
    public ActionResult<UserResponseDto> Update(int id, [FromBody] UserRequestDto dto)
    {
        try
        {
            var user = uService.Update(id, dto);
            return Ok(user);
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
            uService.Delete(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}/salary")]
    public ActionResult<UserResponseDto> UpdateSalary(int id, [FromBody] SalaryRequestDto dto)
    {
        try
        {
            dto.UserId = id;
            var user = uService.UpdateSalary(id, dto);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("department/{departmentId}")]
    public ActionResult<List<UserResponseDto>> GetByDepartmentId(int departmentId)
    {
        try
        {
            var users = uService.GetByDepartmentId(departmentId);
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("email/{email}")]
    public ActionResult<UserResponseDto> GetByEmail(string email)
    {
        try
        {
            var user = uService.GetByEmail(email);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("username/{username}")]
    public ActionResult<UserResponseDto> GetByUsername(string username)
    {
        try
        {
            var user = uService.GetByUsername(username);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}