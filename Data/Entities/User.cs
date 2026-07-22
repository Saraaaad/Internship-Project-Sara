public class User : EntityTracker
{
    public int Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public Role Role { get; private set; }
    public string? SerialNumber { get; private set; }
    public string FullName { get; private set; }
    public string? Phone { get; private set; }
    public Salary? Salary { get; private set; }
    public int? DepartmentId { get; private set; }
    public string? LeadSerialNumber { get; private set; }
    public Department Department { get; set; }
    public List<Tasks> Tasks { get; set; }
    public List<Note> Notes { get; set; }
    private User() : base(0)
    {
        Tasks = new List<Tasks>();
        Notes = new List<Note>();
    }
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

    public void UpdateProfile(string? fullName, string? email, string? phone)
    {
        FullName = fullName ?? FullName;
        Email = email ?? Email;
        Phone = phone ?? Phone;
        UpdatedAt = DateTime.UtcNow;
    }
    public void ChangeRole(Role newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignDepartment(int departmentId)
    {
        DepartmentId = departmentId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSalary(decimal amount, decimal bonus, string currency = "USD")
    {
        Salary = new Salary(amount, bonus, currency);
        UpdatedAt = DateTime.UtcNow;
    }
    public void SetSerialNumber(string serialNumber)
    {
        SerialNumber = serialNumber;
        UpdatedAt = DateTime.UtcNow;
    }
    public void SetPassword(string password)
    {
        Password = password;
        UpdatedAt = DateTime.UtcNow;
    }
    public void SetRole(Role role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }
}