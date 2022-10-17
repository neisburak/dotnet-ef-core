using EF.Inheritance.Data;
using EF.Inheritance.Entities;

namespace EF.Inheritance.Services;

public class TablePerHierarchyService
{
    public static void SeedEmployees()
    {
        using var context = new DataContext();

        if (context.Employees.Any()) return;

        context.Employees.Add(new ContractEmployee { FirstName = "John", LastName = "Smith", HourlyPay = 15, HoursWorked = 25 });
        context.Employees.Add(new PermanentEmployee { FirstName = "Adam", LastName = "Wilson", AnnualSalary = 25000 });
        context.Employees.Add(new ContractEmployee { FirstName = "Steven", LastName = "Harris", HourlyPay = 15, HoursWorked = 25 });
        context.Employees.Add(new PermanentEmployee { FirstName = "Robinson", LastName = "Lewis", AnnualSalary = 30000 });
        context.Employees.Add(new ContractEmployee { FirstName = "Taylor", LastName = "Wright", HourlyPay = 15, HoursWorked = 25 });

        context.SaveChanges();
    }

    public static List<PermanentEmployee> GetPermanentEmployees()
    {
        using var context = new DataContext();

        return context.Employees.OfType<PermanentEmployee>().ToList();
    }

    public static List<ContractEmployee> GetContractEmployees()
    {
        using var context = new DataContext();

        return context.ContractEmployees.ToList();
    }
}