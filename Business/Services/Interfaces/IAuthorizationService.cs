public interface IAuthorizationService
{
    bool CanAccessUserData(int userId);
    bool IsAdminOrHR();
    bool IsLead();
    int GetCurrentUserId();
}