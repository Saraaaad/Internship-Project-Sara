using System.ComponentModel.DataAnnotations;

public class TaskRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }
    public string Description { get; set; }
    
    [Required(ErrorMessage = "Employee ID is required")]
    public int EmployeeId { get; set; }
    public Status Status { get; set; }

    [Required(ErrorMessage = "Deadline is required")]
    [DataType(DataType.DateTime, ErrorMessage = "Deadline must be a valid date and time")]
    public DateTime Deadline { get; set; }
}