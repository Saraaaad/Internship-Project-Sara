public class Tasks : EntityTracker
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Status Status { get; set; }
    public DateTime Deadline { get; set; }
    public int EmployeeId { get; set; }
    public int AssignedById { get; set; }

    public Tasks(int id, string title, string description, Status status, DateTime deadline, int employeeId, int assignedById, int createdBy) : base(createdBy)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        Deadline = deadline;
        EmployeeId = employeeId;
        AssignedById = assignedById;
    }

    public override string ToString()
    {
        return $"Tasks [Id={Id}, Title={Title}, Description={Description}, Status={Status}, Deadline={Deadline}, EmployeeId={EmployeeId}]";
    }
}