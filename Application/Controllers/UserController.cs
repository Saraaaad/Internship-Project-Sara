using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService uService;
    private readonly IAuthorizationService authzService;
    private readonly ILogger<UserController> logger;
    private readonly ILogService logService;

    public UserController(IUserService userService, IAuthorizationService authorizationService, ILogger<UserController> logger, ILogService logService)
    {
        uService = userService;
        authzService = authorizationService;
        this.logger = logger;
        this.logService = logService;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public ActionResult<List<UserResponseDto>> GetAll()
    {
        var users = uService.GetAll();
        return Ok(users);
    }

    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<UserResponseDto> GetById(int id)
    {
        if (!authzService.CanAccessUserData(id)){
            logService.Log(LogLevel.Warning, $"User with id: {authzService.GetCurrentUserId()} tried to access data for user with id : {id}");
            logger.LogWarning("User with id: {currentId} tried to access data for user with id : {id}" , authzService.GetCurrentUserId() ,id);
            return Forbid();
        }
        var user = uService.GetById(id);
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public ActionResult<UserResponseDto> Create([FromBody] UserRequestDto dto)
    {
        var user = uService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }
    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}")]
    public ActionResult<UserResponseDto> Update(int id, [FromBody] UserRequestDto dto)
    {
        var user = uService.Update(id, dto);
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        uService.Delete(id);
        return NoContent();
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}/salary")]
    public ActionResult<UserResponseDto> UpdateSalary(int id, [FromBody] SalaryRequestDto dto)
    {
        dto.UserId = id;
        var user = uService.UpdateSalary(id, dto);
        return Ok(user);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("department/{departmentId}")]
    public ActionResult<List<UserResponseDto>> GetByDepartmentId(int departmentId)
    {
        var users = uService.GetByDepartmentId(departmentId);
        return Ok(users);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("email/{email}")]
    public ActionResult<UserResponseDto> GetByEmail(string email)
    {
        var user = uService.GetByEmail(email);
        return Ok(user);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("username/{username}")]
    public ActionResult<UserResponseDto> GetByUsername(string username)
    {
        var user = uService.GetByUsername(username);
        return Ok(user);
    }
}