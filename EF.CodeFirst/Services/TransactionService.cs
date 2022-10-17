using EF.CodeFirst.Data;
using EF.CodeFirst.Entities;

namespace EF.CodeFirst.Services;

public class TransactionService
{
    public static bool PerformTransaction(DataContext context)
    {
        using var transaction = context.Database.BeginTransaction();
        try
        {
            var category = new Category { Name = "Clothes" };
            context.Categories.Add(category);
            context.SaveChanges();

            var product = new Product { Name = "Jacket", UnitPrice = 24.99m, CategoryId = int.MaxValue };
            context.Products.Add(product);
            context.SaveChanges();

            transaction.Commit();

            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }
}