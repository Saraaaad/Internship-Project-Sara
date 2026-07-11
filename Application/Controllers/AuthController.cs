using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public ActionResult<RegistrationResponseDto> Register([FromBody] RegistrationRequestDto dto)
    {
        try
        {
            var register = authService.Register(dto);
            return Created($"/api/users/{register.UserId}", register);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<LoginResponseDto> Login([FromBody] LoginRequestDto dto)
    {
        try
        {
            var login = authService.Login(dto);
            return Ok(login);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}