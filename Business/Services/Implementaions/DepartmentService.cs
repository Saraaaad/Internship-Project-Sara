using InternshipProjectSara.Data.Context;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository drepository;
    private readonly DatabaseContext _context;

    public DepartmentService(IDepartmentRepository repository, DatabaseContext context)
    {
        drepository = repository;
        _context = context;
    }

    public DepartmentResponseDto GetById(int id)
    {
        var department = drepository.GetById(id);
        if (department == null)
            throw new NotFoundException("Department", id);

        return department.ToDto<Department, DepartmentResponseDto>();
    }

    public List<DepartmentResponseDto> GetAll()
    {
        var departments = drepository.GetAll();
        return departments.ToDtoList<Department, DepartmentResponseDto>();
    }

    public DepartmentResponseDto Create(DepartmentRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var department = dto.ToEntity<DepartmentRequestDto, Department>();
        var existingDepartment = drepository.GetByName(department.Name);
        if (existingDepartment != null)
            throw new DuplicateException($"Department {dto.Name} already exists");

        drepository.Add(department);
        _context.SaveChanges();
        return department.ToDto<Department, DepartmentResponseDto>();
    }

    public DepartmentResponseDto Update(int id, DepartmentRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var department = drepository.GetById(id);
        if (department == null)
            throw new NotFoundException("Department", id);

        department.Name = dto.Name;
        department.LeadNumber = dto.LeadSerialNumber;

        drepository.Update(department);
        _context.SaveChanges();
        return department.ToDto<Department, DepartmentResponseDto>();
    }

    public void Delete(int id)
    {
        var department = drepository.GetById(id);
        if (department == null)
            throw new NotFoundException("Department", id);

        drepository.Delete(id);
        _context.SaveChanges();
    }

    public DepartmentResponseDto GetByName(string name)
    {
        var department = drepository.GetByName(name);
        if (department == null)
            throw new NotFoundException($"Department with name {name} not found");

        return department.ToDto<Department, DepartmentResponseDto>();
    }
}