namespace EF.Inheritance.Entities;

public class PermanentEmployee : Employee
{
    public decimal AnnualSalary { get; set; }

    public override string ToString() => $"{FirstName} {LastName} - Permanent - {AnnualSalary}";
}