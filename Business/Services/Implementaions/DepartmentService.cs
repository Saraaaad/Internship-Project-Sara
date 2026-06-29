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
            throw new Exception("Department not found");

        return department.ToDto<Department, DepartmentResponseDto>();
    }

    public List<DepartmentResponseDto> GetAll()
    {
        var departments = drepository.GetAll();
        return departments.ToDtoList<Department, DepartmentResponseDto>();
    }

    public DepartmentResponseDto Create(DepartmentRequestDto dto)
    {
        var department = dto.ToEntity<DepartmentRequestDto, Department>();
        if (department == null) 
            throw new ArgumentNullException(nameof(department));
            
        var existingDepartment = drepository.GetByName(department.Name);
        if (existingDepartment != null)
            throw new Exception($"Department with name {department.Name} already exists");

        drepository.Add(department);
        _context.SaveChanges();
        return department.ToDto<Department, DepartmentResponseDto>();
    }

    public DepartmentResponseDto Update(int id, DepartmentRequestDto dto)
    {
        var department = drepository.GetById(id);
        if (department == null) 
            throw new Exception("Department not found");
        
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
            throw new Exception("Department not found");
            
        drepository.Delete(id);
        _context.SaveChanges();
    }

    public DepartmentResponseDto GetByName(string name)
    {
        var department = drepository.GetByName(name);
        if (department == null) 
            throw new Exception("Department not found");
            
        return department.ToDto<Department, DepartmentResponseDto>();
    }
}