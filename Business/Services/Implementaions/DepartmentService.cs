using InternshipProjectSara.Data.Context;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository drepository;
    private readonly DatabaseContext _context;
    private readonly ILogger<DepartmentService> logger;
    private readonly ILogService logService;

    public DepartmentService(IDepartmentRepository repository, DatabaseContext context, ILogger<DepartmentService> logger, ILogService logService)
    {
        drepository = repository;
        _context = context;
        this.logger = logger;
        this.logService = logService;
    }

    public DepartmentResponseDto GetById(int id)
    {
        logger.LogInformation("Retrieving department with ID: {id}", id);
        logService.Log(LogLevel.Information, $"Retrieving department with ID: {id}");
        var department = drepository.GetById(id);
        if (department == null)
        {
            logger.LogWarning("Department with ID: {id} not found", id);
            logService.Log(LogLevel.Warning, $"Department with ID: {id} not found");
            throw new NotFoundException("Department", id);
        }
        logger.LogInformation("Department with ID: {id} retrieved successfully", id);
        logService.Log(LogLevel.Information, $"Department with ID: {id} retrieved successfully");

        return department.ToDto<Department, DepartmentResponseDto>();
    }

    public List<DepartmentResponseDto> GetAll()
    {
        logger.LogInformation("Retrieving all departments");
        logService.Log(LogLevel.Information, $"Retrieving all departments");
        var departments = drepository.GetAll();
        logger.LogInformation("All departments retrieved successfully");
        logService.Log(LogLevel.Information, $"All departments retrieved successfully");
        return departments.ToDtoList<Department, DepartmentResponseDto>();
    }

    public DepartmentResponseDto Create(DepartmentRequestDto dto)
    {
        logger.LogInformation("Creating department");
        logService.Log(LogLevel.Information, $"Creating department");
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var department = dto.ToEntity<DepartmentRequestDto, Department>();
        var existingDepartment = drepository.GetByName(department.Name);
        department.LeadNumber = dto.LeadSerialNumber ?? "";
        if (existingDepartment != null)
        {
            logService.Log(LogLevel.Warning, $"Department with name: {dto.Name} already exists");
            logger.LogWarning("Failed, Department with name: {name} already exists", department.Name);
            throw new DuplicateException($"Department {dto.Name} already exists");
        }
        drepository.Add(department);
        _context.SaveChanges();

        logService.Log(LogLevel.Information, $"Department with ID: {department.Id} created successfully");
        logger.LogInformation("Department with ID: {id} created successfully", department.Id);
        return department.ToDto<Department, DepartmentResponseDto>();
    }

    public DepartmentResponseDto Update(int id, DepartmentRequestDto dto)
    {
        logService.Log(LogLevel.Information, $"Updating department with ID: {id}");
        logger.LogInformation("Updating department with ID: {id}", id);
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var department = drepository.GetById(id);
        if (department == null)
        {
            logService.Log(LogLevel.Warning, $"Department with ID: {id} not found");
            logger.LogWarning("Department with ID: {id} not found", id);
            throw new NotFoundException("Department", id);
        }
        var existingDepartment = drepository.GetByName(dto.Name);
        if (existingDepartment != null && existingDepartment.Id != id)
        {
            logService.Log(LogLevel.Warning, $"Department with name: {dto.Name} already exists");
            logger.LogWarning("Failed, Department with name: {name} already exists", dto.Name);
            throw new DuplicateException($"Department {dto.Name} already exists");
        }
        department.Name = dto.Name;
        department.LeadNumber = dto.LeadSerialNumber;

        drepository.Update(department);
        _context.SaveChanges();

        logService.Log(LogLevel.Information, $"Department with ID: {department.Id} updated successfully");
        logger.LogInformation("Department with ID: {id} updated successfully", department.Id);
        return department.ToDto<Department, DepartmentResponseDto>();
    }

    public void Delete(int id)
    {
        logger.LogInformation("Deleting department with ID: {id}", id);
        logService.Log(LogLevel.Information, $"Deleting department with ID: {id}");
        var department = drepository.GetById(id);
        if (department == null)
        {
            logService.Log(LogLevel.Warning, $"Department with ID: {id} not found");
            logger.LogWarning("Department with ID: {id} not found", id);
            throw new NotFoundException("Department", id);
        }
        drepository.Delete(id);
        _context.SaveChanges();

        logService.Log(LogLevel.Information, $"Department with ID: {id} deleted successfully");
        logger.LogInformation("Department with ID: {id} deleted successfully", id);
    }

    public DepartmentResponseDto GetByName(string name)
    {
        logger.LogInformation("Retrieving department with name: {name}", name);
        logService.Log(LogLevel.Information, $"Retrieving department with name: {name}");
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        var department = drepository.GetByName(name);
        if (department == null)
        {
            logService.Log(LogLevel.Warning, $"Department with name: {name} not found");
            logger.LogWarning("Department with name: {name} not found", name);
            throw new NotFoundException($"Department with name {name} not found");
        }
        logService.Log(LogLevel.Information, $"Department with name: {name} retrieved successfully");
        logger.LogInformation("Department with name: {name} retrieved successfully", name);
        return department.ToDto<Department, DepartmentResponseDto>();
    }
}