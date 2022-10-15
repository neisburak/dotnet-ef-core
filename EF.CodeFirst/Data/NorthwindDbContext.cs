using EF.CodeFirst.Entities;
using EF.CodeFirst.Entities.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Utilities.Extensions;

namespace EF.CodeFirst.Data;

public class NorthwindDbContext : DbContext
{
    private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    public NorthwindDbContext() { }
    public NorthwindDbContext(DbContextOptions<NorthwindDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var options = DbContextExtensions.GetOptions("SqlServer");

        //if (options.LogEnabled) optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        if (options.LogEnabled) optionsBuilder.UseLoggerFactory(_loggerFactory);

        optionsBuilder.UseLazyLoadingProxies().UseSqlServer(options.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Usage of Fluent API
        modelBuilder.Entity<Product>().Property(p => p.UnitPrice).HasPrecision(18, 2); // Default precision is (18, 2). If you want to use with data annotation: [Precision(18, 2)]
                                                                                       // modelBuilder.Entity<Category>().ToTable("CategoryTable", "Products");
        modelBuilder.Entity<Category>().Property(p => p.Name).HasColumnName("CategoryName");
        modelBuilder.Entity<Category>().Property(p => p.Name).IsRequired();
        modelBuilder.Entity<Category>().Property(p => p.Description).IsRequired(false).HasMaxLength(256);

        // Relations
        modelBuilder.Entity<Category>().HasMany(m => m.Products).WithOne(w => w.Category).HasForeignKey(h => h.CategoryId).OnDelete(DeleteBehavior.NoAction); // One to Many Relation
        modelBuilder.Entity<Customer>().HasOne(m => m.Address).WithOne(w => w.Customer).HasForeignKey<CustomerAddress>(f => f.Id).OnDelete(DeleteBehavior.Restrict); // One to One Relation
        modelBuilder.Entity<Course>().HasMany(m => m.Students).WithMany(w => w.Courses).UsingEntity<Dictionary<string, object>>(
            "StudentCourse",
            x => x.HasOne<Student>().WithMany().HasForeignKey("StudentId").HasConstraintName("FK_StudentId"),
            x => x.HasOne<Course>().WithMany().HasForeignKey("CourseId").HasConstraintName("FK_CourseId")
        ); // Many to Many Relation

        // Indexes
        modelBuilder.Entity<Customer>().HasIndex(i => i.Name); // Non-clustered index
        modelBuilder.Entity<Category>().HasIndex(i => new { i.Name, i.Description }); // Composed index
        modelBuilder.Entity<Product>().HasIndex(i => i.Name).IsUnique(true).IncludeProperties(i => new { i.UnitPrice, i.CategoryId });

        // modelBuilder.Entity<SimpleProduct>().ToSqlQuery("SELECT p.Id, p.Name, p.UnitPrice, c.Name as CategoryName FROM Products p JOIN Categories c ON p.CategoryId = c.Id");
        modelBuilder.Entity<ProductByCategory>().HasNoKey().ToView("ProductByCategory");

        // Global Query Filters
        modelBuilder.Entity<Product>().HasQueryFilter(p => p.IsActive && !p.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        ChangeTracker.Entries().ToList().ForEach(f =>
        {
            if (f.Entity is Product product)
            {
                if (f.State == EntityState.Added)
                {
                    product.CreatedOn = DateTime.Now;
                    product.IsActive = true;
                    product.IsDeleted = false;
                }
                else if (f.State == EntityState.Modified)
                {
                    product.ModifiedOn = DateTime.Now;
                }
            }
        });
        return base.SaveChanges();
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductFeature> ProductFeatures => Set<ProductFeature>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<ProductByCategory> ProductsByCategories => Set<ProductByCategory>();
}