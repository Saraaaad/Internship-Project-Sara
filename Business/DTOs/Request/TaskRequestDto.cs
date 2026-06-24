using System.ComponentModel.DataAnnotations;

public class TaskRequestDto
{
    [Required(ErrorMessage = "Title is required")]

    public string Title { get; set; }
    public string Description { get; set; }
    [Required(ErrorMessage = "Employee ID is required")]
    public int EmployeeId { get; set; }
}