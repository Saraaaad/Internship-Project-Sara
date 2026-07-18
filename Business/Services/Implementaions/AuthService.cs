using InternshipProjectSara.Data.Context;

public class AuthService : IAuthService
{
    private readonly IUserRepository userRepository;
    private readonly DatabaseContext context;
    private readonly JwtTokenGenerator tokenGenerator;
    private readonly ILogger<AuthService> logger;
    private readonly ILogService logService;

    public AuthService(IUserRepository userRepository, DatabaseContext context, JwtTokenGenerator tokenGenerator, ILogger<AuthService> logger, ILogService logService)
    {
        this.logger = logger;
        this.userRepository = userRepository;
        this.context = context;
        this.tokenGenerator = tokenGenerator;
        this.logService = logService;
    }

    public RegistrationResponseDto Register(RegistrationRequestDto dto)
    {
        logService.Log(LogLevel.Information, "Attempting to register user");
        logger.LogInformation("Attempting to register user");
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        if (userRepository.GetByEmail(dto.Email) != null)
        {
            logger.LogWarning("Failed, Email {email} already exists", dto.Email);
            logService.Log(LogLevel.Warning, $"Email {dto.Email} already exists");
            throw new DuplicateException($"Email {dto.Email} already exists");
        }
        if (userRepository.GetByUsername(dto.Username) != null)
        {
            logger.LogWarning("Failed, Username {username} already exists", dto.Username);
            logService.Log(LogLevel.Warning, $"Username {dto.Username} already exists");
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
        logService.Log(LogLevel.Information, "User registered successfully");
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
        logService.Log(LogLevel.Information, "Attempting to log in user");
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var user = userRepository.GetByUsername(dto.Username);
        if (user == null)
        {
            logger.LogWarning("Failed, User with username {username} not found", dto.Username);
            logService.Log(LogLevel.Warning, $"User with username {dto.Username} not found");
            throw new NotFoundException("User not found");
        }
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
        {
            logService.Log(LogLevel.Warning, $"Invalid password for user {dto.Username}");
            logger.LogWarning("Failed, Invalid password for user {username}", dto.Username);
            throw new UnauthorizedAccessException("Invalid username or password");
        }
        var token = tokenGenerator.GenerateToken(user);
        logger.LogInformation("User {username} logged in successfully", dto.Username);
        logService.Log(LogLevel.Information, $"User {dto.Username} logged in successfully");

        return new LoginResponseDto
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            UserId = user.Id
        };
    }
}