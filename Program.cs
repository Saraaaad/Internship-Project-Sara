using InternshipProjectSara.Data.Context;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

Mapper.Initialize();

builder.Services.AddSingleton<JwtSettings>(sp => builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>());
builder.Services.AddSingleton<JwtTokenGenerator>();


// Connect to Database
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repos
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>)); // Generic Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Add Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// Authentication and Authorization
builder.Services.AddHttpContextAccessor(); 
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddJwtAuthentication(builder.Configuration);

// Add Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Create Database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    context.Database.EnsureCreated();
}

// init GlobalExceptionHandler
GlobalExceptionHandler.Configure(app);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();