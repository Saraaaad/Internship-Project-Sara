public class NoteResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public int IssuerId { get; set; }
    public string IssuerName { get; set; }
    public DateTime CreatedAt { get; set; }
}