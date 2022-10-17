using EF.Inheritance.Services;

// Table per Hierarchy
TablePerHierarchyService.SeedEmployees();

var permanentEmployees = TablePerHierarchyService.GetPermanentEmployees();
WriteItems(permanentEmployees);

var contractEmployees = TablePerHierarchyService.GetContractEmployees();
WriteItems(contractEmployees);



// Table per Type
TablePerTypeService.SeedBillingDetails();

var creditCards = TablePerTypeService.GetCreditCards();
WriteItems(creditCards);

var bankAccounts = TablePerTypeService.GetBankAccounts();
WriteItems(bankAccounts);



// Helper Methods
void WriteItems<T>(List<T> items) where T : new() => items.ForEach(e => Console.WriteLine(e));