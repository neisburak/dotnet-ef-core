namespace EF.Inheritance.Entities;

public class ContractEmployee : Employee
{
    public decimal HourlyPay { get; set; }
    public int HoursWorked { get; set; }

    public override string ToString() => $"{FirstName} {LastName} - Contract - {HourlyPay * HoursWorked}";
}