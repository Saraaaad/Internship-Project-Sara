using Microsoft.EntityFrameworkCore;
using InternshipProjectSara.Data.Context;

public static class ProgramDI
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        Mapper.Initialize();

        // Singleton:
        services.AddSingleton(config.GetSection("JwtSettings").Get<JwtSettings>()!);
        services.AddSingleton<JwtTokenGenerator>();

        // Scoped:
        // Connect to Database
        services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        // Add Repos
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<INoteRepository, NoteRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ILogRepository, LogRepository>();

        // Add Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<INoteService, NoteService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ILogService, LogService>();

        // Authentication and Authorization
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddJwtAuthentication(config);

        services.AddControllers(); // Add Controllers
        services.AddSwagger(); // Add Swagger config

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", policy =>
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials());
        });
       

        return services;
    }
}