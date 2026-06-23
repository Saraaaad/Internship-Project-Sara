using System.ComponentModel.DataAnnotations;

public class UserRequestDto
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Role is required")]
    [EnumDataType(typeof(Role), ErrorMessage = "Invalid role")]
    public Role Role { get; set; }

    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string FullName { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; }
    public int? DepartmentId { get; set; }
    public decimal SalaryAmount { get; set; }
    public decimal SalaryBonus { get; set; }
    public string SalaryCurrency { get; set; }
}