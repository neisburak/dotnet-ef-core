using EF.DatabaseFirst.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Utilities.Extensions;

namespace EF.DatabaseFirst.Data;

public class NorthwindDbContext : DbContext
{
    public NorthwindDbContext() { }

    public NorthwindDbContext(DbContextOptions<NorthwindDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var options = DbContextExtensions.GetOptions("SqlServer");

        if (options.LogEnabled) optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);

        optionsBuilder.UseSqlServer(options.ConnectionString);
    }

    public DbSet<Product> Products => Set<Product>();
}
