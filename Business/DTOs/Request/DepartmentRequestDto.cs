using System.ComponentModel.DataAnnotations;

public class DepartmentRequestDto
{
    [Required(ErrorMessage = "Department name is required")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Department code is required")]
    public string DepartmentCode { get; set; }
    public string? LeadSerialNumber { get; set; }
}