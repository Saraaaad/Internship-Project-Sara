public class Department : EntityTracker{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DepartmentCode { get; set; }

    public string LeadNumber { get; set; }
    public List<User> Employees { get; set; }
    private Department() : base(0)
    {
        Employees = new List<User>();
    }

    public Department(int id, string name, string departmentCode, string leadNumber, List<User> employees, int createdBy) : base(createdBy)
    {
        Id = id;
        Name = name;
        DepartmentCode = departmentCode;
        LeadNumber = leadNumber;
        Employees = employees;
    }

    public override string ToString()
    {
        return $"Department [Id={Id}, Name={Name}, DepartmentCode={DepartmentCode}, LeadNumber={LeadNumber}, Employees={string.Join(", ", Employees)}]";
    }
}