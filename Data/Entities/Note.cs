public class Note : EntityTracker
{

    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int EmployeeId { get; set; }
    public int IssuerId { get; set; }
    private Note() : base(0) { }
    public Note(int id, string title, string content, int employeeId, int createdBy) : base(createdBy)
    {

        Id = id;
        Title = title;
        Content = content;
        EmployeeId = employeeId;
    }

    public override string ToString()
    {
        return $"Note [Id={Id}, Title={Title}, Content={Content}, EmployeeId={EmployeeId}]";
    }
}