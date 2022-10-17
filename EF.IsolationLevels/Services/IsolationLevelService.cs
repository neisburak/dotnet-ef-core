using System.Data;
using EF.CodeFirst.Data;
using EF.CodeFirst.Entities;
using EF.IsolationLevels.Models;
using Microsoft.EntityFrameworkCore;

namespace EF.IsolationLevels.Services;

public class IsolationLevelService
{
    public static void PerformOperation(Action<TransactionStage, int> action)
    {
        using var context = new DataContext();
        using var transaction = context.Database.BeginTransaction();
        try
        {
            var product = context.Products.First();

            product.UnitPrice = new Random().Next(1000, 5000);

            context.Products.Add(new Product
            {
                Name = $"Test-{new Random().Next(1000, 9999)}",
                Category = context.Categories.First(),
                UnitPrice = new Random().Next(100, 999) / 10,
            });

            context.SaveChanges();

            action(TransactionStage.BeforeCommit, product.Id);

            Console.Write("Press any key to commit.");
            Console.ReadKey();
            Console.WriteLine();

            transaction.Commit();

            action(TransactionStage.AfterCommit, product.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            transaction.Rollback();
        }
    }

    public static void PerformOperationWithListing(IsolationLevel isolationLevel, Action<TransactionStage, int> action)
    {
        using var context = new DataContext();
        using var transaction = context.Database.BeginTransaction(isolationLevel);
        try
        {
            var products = context.Products.ToList();
            products.ForEach(f => Console.WriteLine(f));

            action(TransactionStage.BeforeCommit, products.First().Id);

            var repeatableProducts = context.Products.ToList();
            repeatableProducts.ForEach(f => Console.WriteLine(f));

            Console.Write("Press any key to commit.");
            Console.ReadKey();

            transaction.Commit();

            action(TransactionStage.AfterCommit, products.First().Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            transaction.Rollback();
        }
    }
}