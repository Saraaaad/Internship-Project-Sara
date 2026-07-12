public abstract class EntityTracker
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    protected EntityTracker() { }

    protected EntityTracker(int createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
}