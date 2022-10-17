using System.Data;
using EF.CodeFirst.Data;
using EF.CodeFirst.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EF.CodeFirst.Services;

public class StoredProcedureService
{
    public static List<Product> GetProductsWithProcedure(DataContext context)
    {
        var category = context.Categories.FirstOrDefault();
        if (category is null) return new List<Product>();

        var param = new SqlParameter("@CategoryId", category.Id);
        return context.Products.FromSqlRaw("EXEC GetProductsByCategory @CategoryId", param).IgnoreQueryFilters().ToList();
    }

    public static int? InsertCategoryWithProcedure(DataContext context)
    {
        if (!context.Categories.Any(a => a.Name == "Phones")) // Prevent duplicate
        {
            var categoryToAdd = new Category { Name = "Phones", Description = "Lorem ipsum dolor sit amet." };
            var outParam = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var result = context.Database.ExecuteSqlInterpolated($"EXEC InsertCategory {categoryToAdd.Name}, {categoryToAdd.Description}, {outParam} OUT");
            return (int?)outParam.Value;
        }
        return null;
    }
}