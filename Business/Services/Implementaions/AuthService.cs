using InternshipProjectSara.Data.Context;

public class AuthService : IAuthService
{
    private readonly IUserRepository userRepository;
    private readonly DatabaseContext context;
    private readonly JwtTokenGenerator tokenGenerator;
    private readonly ILogger<AuthService> logger;

    public AuthService(IUserRepository userRepository, DatabaseContext context, JwtTokenGenerator tokenGenerator, ILogger<AuthService> logger)
    {
        this.logger = logger;
        this.userRepository = userRepository;
        this.context = context;
        this.tokenGenerator = tokenGenerator;
    }

    public RegistrationResponseDto Register(RegistrationRequestDto dto)
    {
        logger.LogInformation("Attempting to register user");
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        if (userRepository.GetByEmail(dto.Email) != null){
            logger.LogWarning("Failed, Email {email} already exists", dto.Email);
            throw new DuplicateException($"Email {dto.Email} already exists");
        }
        if (userRepository.GetByUsername(dto.Username) != null){
            logger.LogWarning("Failed, Username {username} already exists", dto.Username);
            throw new DuplicateException($"Username {dto.Username} already exists");
        }
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = dto.ToEntity<RegistrationRequestDto, User>();
        logger.LogInformation("Creating user");
        user.SetPassword(hashedPassword);
        user.SetRole(Role.Employee);

        userRepository.Add(user);
        context.SaveChanges();

        logger.LogInformation("User registered successfully");
        return new RegistrationResponseDto
        {
            Success = true,
            Message = "Registration successful",
            UserId = user.Id
        };
    }

    public LoginResponseDto Login(LoginRequestDto dto)
    {
        logger.LogInformation("Attempting to log in user");
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var user = userRepository.GetByUsername(dto.Username);
        if (user == null)
        {
            logger.LogWarning("Failed, User with username {username} not found", dto.Username);
            throw new NotFoundException("User not found");
        }
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password)){
            logger.LogWarning("Failed, Invalid password for user {username}", dto.Username);
            throw new UnauthorizedAccessException("Invalid username or password");
        }
        var token = tokenGenerator.GenerateToken(user);
        logger.LogInformation("User {username} logged in successfully", dto.Username);
        return new LoginResponseDto
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            UserId = user.Id
        };
    }
}