public class Note {
    public int Id { get; private set; }

    public string Title { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int EmployeeId { get; private set; }
    public int IssuerId { get; private set; }

    public Note(int id, string title, string content, DateTime createdAt, int employeeId)
    {
        Id = id;
        Title = title;
        Content = content;
        CreatedAt = createdAt;
        EmployeeId = employeeId;
    }

    public override string ToString()
    {
        return $"Note [Id={Id}, Title={Title}, Content={Content}, CreatedAt={CreatedAt}, EmployeeId={EmployeeId}]";
    }
}