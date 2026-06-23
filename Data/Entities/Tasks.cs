public class Tasks
{
    public int Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public Status Status { get; private set; }
    public int EmployeeId { get; private set; }
    public int AssignedById { get; private set; }

    public Tasks(int id, string title, string description, Status status, int employeeId)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        EmployeeId = employeeId;
    }

    public override string ToString()
    {
        return $"Tasks [Id={Id}, Title={Title}, Description={Description}, Status={Status}, EmployeeId={EmployeeId}]";
    }
}