using EF.CodeFirst.Data;
using EF.CodeFirst.Entities;

namespace EF.IsolationLevels.Services;

public class UtilityService
{
    public static async Task SeedContext()
    {
        using var context = new DataContext();

        if (context.Categories.Any()) return;

        var categories = new List<Category>
        {
            new() { Name = "Books", Description = "Lorem ipsum dolor sit amet." },
            new() { Name = "Computers", Description = "Lorem ipsum dolor sit amet." },
        };
        var products = new List<Product>
        {
            new() { Name = "Ulysses", UnitPrice = 15.66m, Category = categories.First() },
            new() { Name = "It starts with us", UnitPrice = 25.66m, Category = categories.First() },
            new() { Name = "Harry Potter", UnitPrice = 16.66m, Category = categories.First() },
            new() { Name = "Atomic Habits", UnitPrice = 24.99m, Category = categories.First() },
            new() { Name = "MacBook", UnitPrice = 315.66m, Category = categories.Last() },
            new() { Name = "Laptop", UnitPrice = 215.66m, Category = categories.Last() },
        };
        await context.AddRangeAsync(products);

        context.SaveChanges();
    }
}