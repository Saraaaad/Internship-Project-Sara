using InternshipProjectSara.Data.Context;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository drepository;
    private readonly DatabaseContext _context;
    private readonly ILogger<DepartmentService> logger;

    public DepartmentService(IDepartmentRepository repository, DatabaseContext context, ILogger<DepartmentService> logger)
    {
        drepository = repository;
        _context = context;
        this.logger = logger;
    }

    public DepartmentResponseDto GetById(int id)
    {
        logger.LogInformation("Retrieving department with ID: {id}", id);
        var department = drepository.GetById(id);
        if (department == null)
        {
            logger.LogWarning("Department with ID: {id} not found", id);
            throw new NotFoundException("Department", id);
        }
        logger.LogInformation("Department with ID: {id} retrieved successfully", id);
        return department.ToDto<Department, DepartmentResponseDto>();
    }

    public List<DepartmentResponseDto> GetAll()
    {
        logger.LogInformation("Retrieving all departments");
        var departments = drepository.GetAll();
        logger.LogInformation("All departments retrieved successfully");
        return departments.ToDtoList<Department, DepartmentResponseDto>();
    }

    public DepartmentResponseDto Create(DepartmentRequestDto dto)
    {
        logger.LogInformation("Creating department");
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var department = dto.ToEntity<DepartmentRequestDto, Department>();
        var existingDepartment = drepository.GetByName(department.Name);
        if (existingDepartment != null){
            logger.LogWarning("Failed, Department with name: {name} already exists", department.Name);
            throw new DuplicateException($"Department {dto.Name} already exists");
        }
        drepository.Add(department);
        _context.SaveChanges();
        logger.LogInformation("Department with ID: {id} created successfully", department.Id);
        return department.ToDto<Department, DepartmentResponseDto>();
    }

    public DepartmentResponseDto Update(int id, DepartmentRequestDto dto)
    {
        logger.LogInformation("Updating department with ID: {id}", id);
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var department = drepository.GetById(id);
        if (department == null)
        {
            logger.LogWarning("Department with ID: {id} not found", id);
            throw new NotFoundException("Department", id);
        }
        var existingDepartment = drepository.GetByName(dto.Name);
        if (existingDepartment != null && existingDepartment.Id != id)
        {
            logger.LogWarning("Failed, Department with name: {name} already exists", dto.Name);
            throw new DuplicateException($"Department {dto.Name} already exists");
        }
        department.Name = dto.Name;
        department.LeadNumber = dto.LeadSerialNumber;

        drepository.Update(department);
        _context.SaveChanges();
        logger.LogInformation("Department with ID: {id} updated successfully", department.Id);
        return department.ToDto<Department, DepartmentResponseDto>();
    }

    public void Delete(int id)
    {
        logger.LogInformation("Deleting department with ID: {id}", id); 
        var department = drepository.GetById(id);
        if (department == null){
            logger.LogWarning("Department with ID: {id} not found", id);
            throw new NotFoundException("Department", id);
        }
        drepository.Delete(id);
        _context.SaveChanges();
        logger.LogInformation("Department with ID: {id} deleted successfully", id);
    }

    public DepartmentResponseDto GetByName(string name)
    {
        logger.LogInformation("Retrieving department with name: {name}", name);
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        var department = drepository.GetByName(name);
        if (department == null)
        {
            logger.LogWarning("Department with name: {name} not found", name);
            throw new NotFoundException($"Department with name {name} not found");
        }
        logger.LogInformation("Department with name: {name} retrieved successfully", name);
        return department.ToDto<Department, DepartmentResponseDto>();
    }
}