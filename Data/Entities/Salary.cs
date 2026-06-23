public class Salary {
    public decimal Amount { get; private set; }
    public decimal Bonus { get; private set; }
    public string Currency { get; private set; }
    public int EmployeeId { get; private set; }

    public Salary(decimal amount, decimal bonus, string currency, int employeeId)
    {
        Amount = amount;
        Bonus = bonus;
        Currency = currency;
        EmployeeId = employeeId;
    }

    public override string ToString()
    {
        return $"Salary [Amount={Amount}, Bonus={Bonus}, Currency={Currency}, EmployeeId={EmployeeId}]";
    }
}