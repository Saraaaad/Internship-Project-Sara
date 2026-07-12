using System.Security.Claims;
public class AuthorizationService : IAuthorizationService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private ClaimsPrincipal User => httpContextAccessor.HttpContext?.User;

    public AuthorizationService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public int GetCurrentUserId()
{
    var id = User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User ID claim is missing.");

    return int.Parse(id);
}

public bool IsAdminOrHR()
{
    return User.IsInRole("Admin") || User.IsInRole("HR");
}

public bool IsLead()
{
    return User.IsInRole("Lead");
}

public bool CanAccessUserData(int targetUserId)
{
    return IsAdminOrHR() || GetCurrentUserId() == targetUserId;
}
}