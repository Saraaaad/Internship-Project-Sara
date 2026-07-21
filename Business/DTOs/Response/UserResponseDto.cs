public class UserResponseDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string SerialNumber { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
     public decimal SalaryAmount { get; set; }
    public decimal SalaryBonus { get; set; }
    public string TotalSalary { get; set; }
    public string SalaryCurrency { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TaskResponseDto>? Tasks { get; set; }
    public List<NoteResponseDto>? Notes { get; set; }
}