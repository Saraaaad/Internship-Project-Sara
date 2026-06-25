public interface IDepartmentRepository
{
    Department GetById(int id);
    List<Department> GetAll();
    void Add(Department department);
    void Update(Department department);
    void Delete(int id);
    }