using EF.Inheritance.Data;
using EF.Inheritance.Entities;

namespace EF.Inheritance.Services;

public class TablePerTypeService
{
    public static void SeedBillingDetails()
    {
        using var context = new DataContext();

        if (context.BillingDetails.Any()) return;

        context.BillingDetails.Add(new BankAccount { Owner = "John Smith", Number = "00000001", BankName = "EF Bank", Swift = "AAAA BB CC DDD" });
        context.BillingDetails.Add(new CreditCard { Owner = "Adam Wilson", Number = "4444 3333 2222 1111", CardType = 1, ExpiryMonth = "07", ExpiryYear = "22" });
        context.BillingDetails.Add(new CreditCard { Owner = "Steven Harris", Number = "4444 3333 1111 2222", CardType = 2, ExpiryMonth = "09", ExpiryYear = "21" });
        context.BillingDetails.Add(new BankAccount { Owner = "Robinson Lewis", Number = "00000002", BankName = "EF Bank", Swift = "AAAA EE FF GGG" });
        context.BillingDetails.Add(new CreditCard { Owner = "Taylor Wright", Number = "4444 1111 3333 2222", CardType = 3, ExpiryMonth = "15", ExpiryYear = "23" });

        context.SaveChanges();
    }

    public static List<CreditCard> GetCreditCards()
    {
        using var context = new DataContext();

        return context.BillingDetails.OfType<CreditCard>().ToList();
    }

    public static List<BankAccount> GetBankAccounts()
    {
        using var context = new DataContext();

        return context.BankAccounts.ToList();
    }
}