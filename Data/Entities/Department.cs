public class Department : EntityTracker{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LeadNumber { get; set; }
    public List<User> Employees { get; set; }

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