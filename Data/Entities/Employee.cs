public class Employee {
    public int Id { get; private set; }
    public string SerialNumber { get; private set; }
    public string FullName { get; private set; }
    public string Phone { get; private set; }
    public Salary Salary { get; private set; }
    public int DepartmentId { get; private set; }
    public int? UserId { get; private set; }
    public string? LeadSerialNumber { get; private set; }
    public List<Tasks> Tasks { get; private set; }
    public List<Note> Notes { get; private set; }

    
    public Employee(int id, string serialNumber, string fullName, string phone, Salary salary, int departmentId, int? userId, string? leadSerialNumber, List<Tasks> tasks, List<Note> notes)
    {
        Id = id;
        SerialNumber = serialNumber;
        FullName = fullName;
        Phone = phone;
        Salary = salary;
        DepartmentId = departmentId;
        UserId = userId;
        LeadSerialNumber = leadSerialNumber;
        Tasks = tasks;
        Notes = notes;
    }

    public override string ToString()
    {
        return $"Employee [Id={Id}, SerialNumber={SerialNumber}, FullName={FullName}, Phone={Phone}, Salary={Salary}, DepartmentId={DepartmentId}, UserId={UserId}, LeadSerialNumber={LeadSerialNumber}, Tasks={string.Join(", ", Tasks)}, Notes={string.Join(", ", Notes)}]";
    }
}