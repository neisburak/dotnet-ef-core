using System.Data;
using EF.IsolationLevels.Data;
using EF.IsolationLevels.Entities;
using EF.IsolationLevels.Services;
using Microsoft.EntityFrameworkCore;

await UtilityService.SeedContext();

bool performUncommitted = false,
     performCommitted = false,
     performNonrepeatable = false,
     performSerializable = false,
     performSnapshot = true;

// ReadUncommitted
if (performUncommitted)
{
    Console.WriteLine(IsolationLevel.ReadUncommitted);
    IsolationLevelService.PerformOperation((stage, id) =>
    {
        Console.WriteLine(stage);
        using var context = new DataContext();
        using var transaction = context.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
        try
        {
            var products = context.Products.ToList();
            products.ForEach(f => Console.WriteLine($"{(id == f.Id ? "*" : ".")} - {f}"));

            transaction.Commit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            transaction.Rollback();
        }
    });
}


// ReadCommitted
if (performCommitted)
{
    Console.WriteLine(IsolationLevel.ReadCommitted);
    IsolationLevelService.PerformOperation((stage, id) =>
    {
        Console.WriteLine(stage);
        using var context = new DataContext();
        using var transaction = context.Database.BeginTransaction(IsolationLevel.ReadCommitted);
        try
        {
            var products = context.Products.ToList();
            products.ForEach(f => Console.WriteLine($"{(id == f.Id ? "*" : ".")} - {f}"));

            transaction.Commit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            transaction.Rollback();
        }
    });
}

// RepeatableRead
if (performNonrepeatable)
{
    Console.WriteLine(IsolationLevel.RepeatableRead);
    IsolationLevelService.PerformOperationWithListing(IsolationLevel.RepeatableRead, (stage, id) =>
    {
        Console.WriteLine(stage);
        using var context = new DataContext();
        context.Database.SetCommandTimeout(2);
        using var transaction = context.Database.BeginTransaction();
        try
        {
            var product = context.Products.Find(id);
            product!.UnitPrice = new Random().Next(1000, 5000);

            context.SaveChanges();

            transaction.Commit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message} {(ex.InnerException is not null ? ex.InnerException.Message : string.Empty)}");
            transaction.Rollback();
        }
    });
}

// Serializable
if (performSerializable)
{
    Console.WriteLine(IsolationLevel.Serializable);
    IsolationLevelService.PerformOperationWithListing(IsolationLevel.Serializable, (stage, id) =>
    {
        Console.WriteLine(stage);
        using var context = new DataContext();
        context.Database.SetCommandTimeout(2);
        using var transaction = context.Database.BeginTransaction();
        try
        {
            var category = context.Categories.First();
            var product = new Product { Name = "Serializable", Category = category, UnitPrice = 500m };
            context.Products.Add(product);
            context.SaveChanges();

            transaction.Commit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message} {(ex.InnerException is not null ? ex.InnerException.Message : string.Empty)}");
            transaction.Rollback();
        }
    });
}

// Snapshot
if (performSnapshot)
{
    Console.WriteLine(IsolationLevel.Snapshot);
    IsolationLevelService.PerformOperationWithListing(IsolationLevel.Snapshot, (stage, id) =>
    {
        Console.WriteLine(stage);
        using var context = new DataContext();
        context.Database.SetCommandTimeout(2);
        using var transaction = context.Database.BeginTransaction();
        try
        {
            var category = context.Categories.First();
            var product = new Product { Name = "Snapshot", Category = category, UnitPrice = 500m };
            context.Products.Add(product);
            context.SaveChanges();

            transaction.Commit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message} {(ex.InnerException is not null ? ex.InnerException.Message : string.Empty)}");
            transaction.Rollback();
        }
    });
}