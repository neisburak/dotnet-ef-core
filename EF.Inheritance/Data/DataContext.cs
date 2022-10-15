using EF.Inheritance.Entities;
using EF.Inheritance.Entities.KeylessEntityTypes;
using EF.Inheritance.Entities.OwnedEntityTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Utilities.Extensions;

namespace EF.Inheritance.Data;

public class DataContext : DbContext
{
    public DataContext() { }
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var options = DbContextExtensions.GetOptions("SqlServer");

        if (options.LogEnabled) optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);

        optionsBuilder.UseSqlServer(options.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Table per Type
        modelBuilder.Entity<BillingDetail>().ToTable("BillingDetails");
        modelBuilder.Entity<CreditCard>().ToTable("CreditCards");
        modelBuilder.Entity<BankAccount>().ToTable("BankAccounts");

        // Owned Entity Types
        modelBuilder.Entity<Student>().OwnsOne(o => o.User);
        modelBuilder.Entity<Teacher>().OwnsOne(o => o.User, e =>
        {
            e.Property(p => p.FirstName).HasColumnName("FirstName");
            e.Property(p => p.LastName).HasColumnName("LastName");
        });

        // Keyless Entity Types
        modelBuilder.Entity<EmployeeCount>().HasNoKey().ToView("EmployeeCount");

        base.OnModelCreating(modelBuilder);
    }

    #region Table Per Hierarchy
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<PermanentEmployee> PermanentEmployees => Set<PermanentEmployee>();
    public DbSet<ContractEmployee> ContractEmployees => Set<ContractEmployee>();
    #endregion

    #region Table Per Type
    public DbSet<BillingDetail> BillingDetails => Set<BillingDetail>();
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    #endregion

    #region Owned Entity Types
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    #endregion

    #region Keyless Entity Types
    public DbSet<EmployeeCount> EmployeeCounts => Set<EmployeeCount>();
    #endregion
}