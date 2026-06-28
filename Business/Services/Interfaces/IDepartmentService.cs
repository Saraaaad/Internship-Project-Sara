public interface IDepartmentService
{
    DepartmentResponseDto GetById(int id);
    List<DepartmentResponseDto> GetAll();
    DepartmentResponseDto Create(DepartmentRequestDto dto);
    DepartmentResponseDto Update(int id, DepartmentRequestDto dto);
    void Delete(int id);
    DepartmentResponseDto GetByName(string name);
}