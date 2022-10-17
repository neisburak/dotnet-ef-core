using EF.CodeFirst.Data;
using EF.CodeFirst.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EF.CodeFirst.Services;

public class RawSQLService
{
    public static List<Customer> GetCustomersFromSqlRaw(DataContext context) => context.Customers.FromSqlRaw<Customer>("SELECT * FROM Customers").ToList();

    public static Product? GetProductFromSqlRaw(DataContext context)
    {
        var param = new SqlParameter("@id", 1);
        return context.Products.FromSqlRaw("SELECT * FROM Products WHERE Id = @id", param).FirstOrDefault();
    }

    public static Category? GetCategoryFromSqlInterpolated(DataContext context, int id)
    {
        return context.Categories.FromSqlInterpolated($"SELECT * FROM Products.CategoryTable WHERE Id = {id}").FirstOrDefault();
    }
}