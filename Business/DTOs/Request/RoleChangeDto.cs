using System.ComponentModel.DataAnnotations;

public class RoleChangeDto
{
    [Required]
    public Role Role { get; set; }
}