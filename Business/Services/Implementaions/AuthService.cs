using InternshipProjectSara.Data.Context;

public class AuthService : IAuthService
{
    private readonly IUserRepository userRepository;
    private readonly DatabaseContext context;
    private readonly JwtTokenGenerator tokenGenerator;

    public AuthService(IUserRepository userRepository, DatabaseContext context, JwtTokenGenerator tokenGenerator)
    {
        this.userRepository = userRepository;
        this.context = context;
        this.tokenGenerator = tokenGenerator;
    }

    public RegistrationResponseDto Register(RegistrationRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (userRepository.GetByEmail(dto.Email) != null)
            throw new DuplicateException($"Email {dto.Email} already exists");

        if (userRepository.GetByUsername(dto.Username) != null)
            throw new DuplicateException($"Username {dto.Username} already exists");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = dto.ToEntity<RegistrationRequestDto, User>();
        user.SetPassword(hashedPassword);
        user.SetRole(Role.Employee);

        userRepository.Add(user);
        context.SaveChanges();

        return new RegistrationResponseDto
        {
            Success = true,
            Message = "Registration successful",
            UserId = user.Id
        };
    }

    public LoginResponseDto Login(LoginRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var user = userRepository.GetByUsername(dto.Username);
        if (user == null)
            throw new NotFoundException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid username or password");

        var token = tokenGenerator.GenerateToken(user);

        return new LoginResponseDto
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            UserId = user.Id
        };
    }
}