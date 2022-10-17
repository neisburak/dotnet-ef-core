using EF.CodeFirst.Data;
using EF.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF.CodeFirst.Services;

public class GlobalQueryFilterService
{
    public static List<Product> GetProducts(DataContext context) => context.Products.Where(w => w.UnitPrice > 20).ToList();
    public static List<Product> GetProductsByIgnoreQueryFilters(DataContext context) => context.Products.IgnoreQueryFilters().Where(w => w.UnitPrice > 20).ToList();
}