public class User : EntityTracker{
    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public Role Role { get; private set; }  
    public string SerialNumber { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public Salary Salary { get; set; }
    public int DepartmentId { get; set; }
    public string? LeadSerialNumber { get; set; }
    public Department Department { get; set; }
    public List<Tasks> Tasks { get; set; }
    public List<Note> Notes { get; set; }

    public User(int id, string username, string email, string password, Role role, string serialNumber, string fullName, string phone, Salary salary, int departmentId, string? leadSerialNumber, Department department, List<Tasks> tasks, List<Note> notes, int createdBy) : base(createdBy)
    {
        Id = id;
        Username = username;
        Email = email;
        Password = password;
        Role = role;
        SerialNumber = serialNumber;
        FullName = fullName;
        Phone = phone;
        Salary = salary;
        DepartmentId = departmentId;
        LeadSerialNumber = leadSerialNumber;
        Department = department;
        Tasks = tasks;
        Notes = notes;
    }

    public override string ToString()
    {
        return $"User [Id={Id}, Username={Username}, Email={Email}, Role={Role}, SerialNumber={SerialNumber}, FullName={FullName}, Phone={Phone}, Salary={Salary}, DepartmentId={DepartmentId}, LeadSerialNumber={LeadSerialNumber}, Department={Department}, Tasks=[{string.Join(", ", Tasks)}], Notes=[{string.Join(", ", Notes)}]]";
    }
}