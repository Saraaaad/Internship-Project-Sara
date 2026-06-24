using System.ComponentModel.DataAnnotations;

public class NoteRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; }
    
    [Required(ErrorMessage = "Employee ID is required")]
    public int EmployeeId { get; set; }
}