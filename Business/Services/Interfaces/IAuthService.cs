public interface IAuthService
{
    RegistrationResponseDto Register(RegistrationRequestDto dto);
    LoginResponseDto Login(LoginRequestDto dto);
}