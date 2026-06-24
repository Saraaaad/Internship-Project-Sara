public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public DateTime CreatedAt { get; set; }
}