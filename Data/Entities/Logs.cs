public class Logs
{
    public int Id { get; set; }
    public LogLevel Level { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}