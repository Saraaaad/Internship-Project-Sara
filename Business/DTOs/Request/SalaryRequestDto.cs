using System.ComponentModel.DataAnnotations;
public class SalaryRequestDto
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    public decimal Bonus { get; set; }
    
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "USD";
}