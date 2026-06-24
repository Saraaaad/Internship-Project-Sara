using System.ComponentModel.DataAnnotations;

public class RegistrationRequestDto
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
    public string FullName { get; set; }
    [Phone(ErrorMessage = "Invalid phone number")]
    public string Phone { get; set; }
}