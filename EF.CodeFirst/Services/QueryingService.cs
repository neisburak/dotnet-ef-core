using EF.CodeFirst.Data;
using EF.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF.CodeFirst.Services;

public class QueryingService
{
    public static Category GetDataWithEagerLoading(DataContext context)
    {
        // It includes Products then its Features. You can use Include and ThenInclude multiple times.
        return context.Categories.Include(i => i.Products).ThenInclude(i => i.Features).First();
    }

    public static Product GetDataWithExplicitLoading(DataContext context)
    {
        var product = context.Products.First();

        if (product is not null)
        {
            context.Entry(product).Reference(r => r.Category).Load(); // Loads single navigation property
            context.Entry(product).Reference(r => r.Category).Query().Where(w => w.Id == 1).Load(); // Usage with Query and Where methods
            context.Entry(product).Collection(c => c.Features).Load(); // Loads collection
        }
        return product!;
    }

    public static List<Product> GetDataWithLazyLoading(DataContext context)
    {
        var category = context.Categories.First();
        return category.Products.ToList();
    }
}