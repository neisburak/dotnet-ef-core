using EF.IsolationLevels.Entities;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;

namespace EF.IsolationLevels.Data;

public class DataContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var options = DbContextExtensions.GetOptions("SqlServer");

        optionsBuilder.UseSqlServer(options.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {        
        modelBuilder.Entity<Category>().HasMany(m => m.Products).WithOne(w => w.Category).HasForeignKey(h => h.CategoryId).OnDelete(DeleteBehavior.NoAction); // One to Many Relation

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
}