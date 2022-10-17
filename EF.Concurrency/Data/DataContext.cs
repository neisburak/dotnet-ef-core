using EF.Concurrency.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF.Concurrency.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(p => p.RowVersion).IsRowVersion();
        modelBuilder.Entity<Product>().Property(p => p.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<Category>().HasMany(m => m.Products).WithOne(w => w.Category).HasForeignKey(h => h.CategoryId).OnDelete(DeleteBehavior.NoAction);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
}