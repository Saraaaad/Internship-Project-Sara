public class Salary {
    public decimal Amount { get; private set; }
    public decimal Bonus { get; private set; }
    public string Currency { get; private set; }

    public Salary(decimal amount, decimal bonus, string currency)
    {
        Amount = amount;
        Bonus = bonus;
        Currency = currency;
    }

    public override string ToString()
    {
        return $"Salary [Amount={Amount}, Bonus={Bonus}, Currency={Currency}]";
    }
}