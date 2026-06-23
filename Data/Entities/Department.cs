public class Department : EntityTracker{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string LeadNumber { get; private set; }
    public List<User> Employees { get; private set; }

    public Department(int id, string name, string leadNumber, List<User> employees, int createdBy) : base(createdBy)
    {
        Id = id;
        Name = name;
        LeadNumber = leadNumber;
        Employees = employees;
    }

    public override string ToString()
    {
        return $"Department [Id={Id}, Name={Name}, LeadNumber={LeadNumber}, Employees={string.Join(", ", Employees)}]";
    }
}