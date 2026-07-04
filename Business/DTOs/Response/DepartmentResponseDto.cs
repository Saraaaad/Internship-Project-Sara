public class DepartmentResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DepartmentCode { get; set; }
    public string? LeadNumber { get; set; }
    public int EmployeeCount { get; set; }
    public List<string> EmployeeNames { get; set; }
}