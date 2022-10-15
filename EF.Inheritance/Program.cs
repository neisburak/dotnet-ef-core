using EF.Inheritance.Data;
using EF.Inheritance.Entities;

using var context = new DataContext();

#region Table per Hierarchy
if (!context.Employees.Any()) SeedEmployees(context);

var permanentEmployees = context.PermanentEmployees.ToList();
PrintPermanentEmployees(permanentEmployees);

var contractEmployees = context.ContractEmployees.ToList();
PrintContractEmployees(contractEmployees);

var castedPermanentEmployees = context.Employees.OfType<PermanentEmployee>().ToList();
PrintPermanentEmployees(castedPermanentEmployees);

void SeedEmployees(DataContext context)
{
    context.Employees.Add(new ContractEmployee { FirstName = "John", LastName = "Smith", HourlyPay = 15, HoursWorked = 25 });
    context.Employees.Add(new PermanentEmployee { FirstName = "Adam", LastName = "Wilson", AnnualSalary = 25000 });
    context.Employees.Add(new ContractEmployee { FirstName = "Steven", LastName = "Harris", HourlyPay = 15, HoursWorked = 25 });
    context.Employees.Add(new PermanentEmployee { FirstName = "Robinson", LastName = "Lewis", AnnualSalary = 30000 });
    context.Employees.Add(new ContractEmployee { FirstName = "Taylor", LastName = "Wright", HourlyPay = 15, HoursWorked = 25 });

    context.SaveChanges();
}

void PrintPermanentEmployees(List<PermanentEmployee> employees) => employees.ForEach(f => Console.WriteLine(f));
void PrintContractEmployees(List<ContractEmployee> employees) => employees.ForEach(f => Console.WriteLine(f));
#endregion

#region Table per Type
if (!context.BillingDetails.Any()) SeedBillingDetails(context);

var creditCards = context.CreditCards.ToList();
PrintCreditCards(creditCards);

var bankAccounts = context.BankAccounts.ToList();
PrintBankAccounts(bankAccounts);

var castedCreditCards = context.BillingDetails.OfType<CreditCard>().ToList();
PrintCreditCards(castedCreditCards);

void SeedBillingDetails(DataContext context)
{
    context.BillingDetails.Add(new BankAccount { Owner = "John Smith", Number = "00000001", BankName = "EF Bank", Swift = "AAAA BB CC DDD" });
    context.BillingDetails.Add(new CreditCard { Owner = "Adam Wilson", Number = "4444 3333 2222 1111", CardType = 1, ExpiryMonth = "07", ExpiryYear = "22" });
    context.BillingDetails.Add(new CreditCard { Owner = "Steven Harris", Number = "4444 3333 1111 2222", CardType = 2, ExpiryMonth = "09", ExpiryYear = "21" });
    context.BillingDetails.Add(new BankAccount { Owner = "Robinson Lewis", Number = "00000002", BankName = "EF Bank", Swift = "AAAA EE FF GGG" });
    context.BillingDetails.Add(new CreditCard { Owner = "Taylor Wright", Number = "4444 1111 3333 2222", CardType = 3, ExpiryMonth = "15", ExpiryYear = "23" });

    context.SaveChanges();
}

void PrintCreditCards(List<CreditCard> details) => details.ForEach(f => Console.WriteLine(f));
void PrintBankAccounts(List<BankAccount> details) => details.ForEach(f => Console.WriteLine(f));
#endregion
