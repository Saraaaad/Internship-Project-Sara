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
        var users = uService.GetAll();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public ActionResult<UserResponseDto> GetById(int id)
    {
        var user = uService.GetById(id);
        return Ok(user);
    }

    [HttpPost]
    public ActionResult<UserResponseDto> Create([FromBody] UserRequestDto dto)
    {
        var user = uService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public ActionResult<UserResponseDto> Update(int id, [FromBody] UserRequestDto dto)
    {
        var user = uService.Update(id, dto);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        uService.Delete(id);
        return NoContent();
    }

    [HttpPut("{id}/salary")]
    public ActionResult<UserResponseDto> UpdateSalary(int id, [FromBody] SalaryRequestDto dto)
    {
        dto.UserId = id;
        var user = uService.UpdateSalary(id, dto);
        return Ok(user);
        
    }

    [HttpGet("department/{departmentId}")]
    public ActionResult<List<UserResponseDto>> GetByDepartmentId(int departmentId)
    {
        var users = uService.GetByDepartmentId(departmentId);
        return Ok(users);
    }

    [HttpGet("email/{email}")]
    public ActionResult<UserResponseDto> GetByEmail(string email)
    {
        var user = uService.GetByEmail(email);
        return Ok(user);
      
    }

    [HttpGet("username/{username}")]
    public ActionResult<UserResponseDto> GetByUsername(string username)
    {
      
        var user = uService.GetByUsername(username);
        return Ok(user);
    }
}