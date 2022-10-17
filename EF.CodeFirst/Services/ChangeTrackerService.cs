using EF.CodeFirst.Data;
using EF.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EF.CodeFirst.Services;

public class ChangeTrackerService
{
    public static List<Product> GetProducts(DataContext context, int page = 1, int pageSize = 10) => context.Products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

    public static List<Product> GetProductsAsNoTracking(DataContext context) => context.Products.AsNoTracking().ToList();

    public static List<EntityEntry> GetTrackedEntities(DataContext context) => context.ChangeTracker.Entries().ToList();
}