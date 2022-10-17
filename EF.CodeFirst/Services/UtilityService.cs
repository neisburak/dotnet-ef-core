using EF.CodeFirst.Data;
using EF.CodeFirst.Entities;

namespace EF.CodeFirst.Services;

public class UtilityService
{
    public static async Task SeedContext(DataContext context)
    {
        if (context.Categories.Any()) return;

        var categories = new List<Category>
        {
            new() { Name = "Books" },
            new() { Name = "Computers" },
        };
        var products = new List<Product>
        {
            new() { Name = "Ulysses", UnitPrice = 15.66m, Category = categories.First(), Features = new List<ProductFeature> { new() { Key = "Paperback", Value = "228" } } },
            new() { Name = "It starts with us", UnitPrice = 25.66m, Category = categories.First(), Features = new List<ProductFeature> { new() { Key = "Paperback", Value = "315" } } },
            new() { Name = "Harry Potter", UnitPrice = 16.66m, Category = categories.First(), Features = new List<ProductFeature> { new() { Key = "Paperback", Value = "412" } } },
            new() { Name = "Atomic Habits", UnitPrice = 24.99m, Category = categories.First(), Features = new List<ProductFeature> { new() { Key = "Paperback", Value = "289" } } },
            new() { Name = "MacBook", UnitPrice = 315.66m, Category = categories.Last(), Features = new List<ProductFeature> { new() { Key = "CPU", Value = "M1" } } },
            new() { Name = "Laptop", UnitPrice = 215.66m, Category = categories.Last(), Features = new List<ProductFeature> { new() { Key = "CPU", Value = "Intel" } } },
        };
        await context.AddRangeAsync(products);

        context.SaveChanges();
    }


}