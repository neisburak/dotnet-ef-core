# .NET 6 Entity Framework Core Examples
Entity Framework is an open-source ORM framework for .NET applications supported by Microsoft. There are several approaches in Entity Framework Core; such as database-first, code-first, model-first.

You can run a dotnet app with environment variables like below. It helps when working with a console app if you have environment-dependent files such as `appsettings.json`.
```
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

## DbContext
Entity Framework applies `Repository` and `Unit of Work` patterns. It performs CRUD operations and transaction management by this way. 

```
public class NorthwindContext : DbContext
{
    public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductFeature> ProductFeatures { get; set; }
} 
```

It has `Add`, `Update`, `Remove`, `Find` and `SaveChanges` methods. There are also several `DbContext` properties too.

| Property | Description
| ----------- | ----------- |
| ChangeTracker | It tracks entities on the memory and allows accessing and making changes to related data.
| ContextId | It is a unique identity assigned to each dbcontext when working with more than one.
| Database | It allows to access or manage connection, command, transaction raw sql queries on it.

### ContextId

We can get `ContextId` of the context like below. (eg dcf8f23e-27fa-4f8a-871d-512a139cb170:0)
```
var id = context.ContextId;
```

### ChangeTracker

Entity Framework Core tracks every and each entity with its state by default. There are five states `Added`, `Modified`, `Deleted`, `Detached`, `Unchanged`. When you perform CRUD operations over entities it marks with related state; and then when the `SaveChanges` method called it changes the state to `Unchanged` or `Detached` (if removed). Default state is `Unchanged`. Untracked entities have `Detached` state. You can get the entity state like below.

```
var state = context.Entry(category).State;
```
Also you can set an entity state manually.

```
context.Entry(category).State = EntityState.Added;
```

When you change the property value of a tracked entity, Entity Framework automatically changes state to `Modified`. You don't need to set it manually or call `Update` method. If your entity's state is `Detached` you have to call `Update` method. Also when you delete a tracked entity it automatically changes the state to `Detached`.

> There is no need to track each data if you don't perform update or delete operations when working with big data such as a million records. In this scenario you should use `AsNoTracking` method. With the usage of this method your data will not be tracked and your application will be more performant by using the memory efficient.

```
var categories = context.Categories.AsNoTracking().ToList();
```

You can access the tracked entities like below.

```
var entries = context.ChangeTracker.Entries();
entries.ToList().ForEach(f =>
{
    if(f.Entity is Category category) Console.WriteLine(category);
});
```

If we use this functionality to perform common operations, it is more appropriate to override `SaveChanges` method in `DbContext` class.

```
public override int SaveChanges()
{
    ChangeTracker.Entries().ToList().ForEach(f =>
    {
        if (f.Entity is Category category)
        {
            if (f.State == EntityState.Added)
            {
                category.CreatedOn = DateTime.Now;
            }
            else if (f.State == EntityState.Modified)
            {
                category.ModifiedOn = DateTime.Now;
            }
        }
    });
    return base.SaveChanges();
}
```

The default tracking behavior can also be change globally with `QueryTrackingBehavior` enum. It has `TrackAll`, `NoTracking`, `NoTrackingWithIdentityResolution` values.

```
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
    optionsBuilder.UseSqlServer(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
```

### Development Approaches
There are three different approaches. These are `Database-First`, `Code-First`, `Model-First`.
Before diving in, you have to install `EF Core Tools` to work with some features such as scaffolding, migrating, etc.

```
dotnet tool install --global dotnet-ef
```

#### Database-First
You need to have an existing database to work with this approach. If you don't, you can run `Northwind.sql` in the Common folder to generate the database first. Then you can create your entities and DbContext. `Microsoft.EntityFrameworkCore.SqlServer` NuGet package must be installed. 

#### Scaffolding
With scaffold you don't have to create your entities, Entity Framewok will create those for you. Both `Microsoft.EntityFrameworkCore.SqlServer` and `Microsoft.EntityFrameworkCore.Design` NuGet packages must be installed.

You can scaffold your database with a connection-string and a provider.

```
dotnet ef dbcontext scaffold "connectionString" Microsoft.EntityFrameworkCore.SqlServer -o Data
```

#### Code-First
With this approach, you simply create your entities and migrate it to your database. Then your database objects will have been created in this way. Both `Microsoft.EntityFrameworkCore.SqlServer` and `Microsoft.EntityFrameworkCore.Design` NuGet packages must be installed.

You can create your migration files like below. After first migration you don't need to specify output directory.

```
dotnet ef migrations add InitialCreate -o Data/Migrations
```

If migrations files created succesfully you have to update your database.

```
dotnet ef database update
```

Your database must be created, notice that `__EFMigrationsHistory` table also created, to track your migration history.

#### Reverting Migrations
If your migration doesn't applied to your database you can simply run the command below to remove migration file.

```
dotnet ef migrations remove
```

> Do not remove your migration files manually.

If your migration applied, you have to update your database to a previous migration first, then you can remove your migration file(s) with the command below.

```
dotnet ef database update InitialCreate
dotnet ef migrations remove
```

### DbSet
It also provides same methods of the `DbContext` object such as `Add`, `Update`, `Remove` etc.

| Method | Description
| ------ | -----------
| Add | Adds an entity to memory with `Added` state. Since adding an object to tracker is expensive there is also `AddAsync` method too. 
| Update | Updates entity state to `Updated` if it's not tracking; otherwise there is no need to call.
| Remove | Updates entity state to `Deleted`.
| AsNoTracking | Prevents tracking entities by the ChangeTracker.
| Find | Finds an entity with given primary key value(s).
| FirstOrDefault | Finds an entity with given predicate, returns `null` if there is no matching data.
| SingleOrDefault | Finds an entity with given predicate, returns `null` if there is no matching data, throws an exception if there are more than one matching data.
| First | Returns the first item of matching data with given predicate, throws an exception if there is no data. 
| Last | Returns the last item of matching data with given predicate. 
| Single | Finds an entity with given predicate, throws an exception if there are more than one matching data or no data.
| Where | Filters the data with given predicate.

### Configurations
There are three ways to configure the tables. Conventions, Fluent API, and Data Annotations in order of best use. If both Data Annotations and Fluent API are applied then Fluent API will be applied.

| Data Annotations | Fluent API | Conventions
| ---------------- | ---------- | -----------
| [Table] | ToTable | DbSet&lt;TEntity&gt;
| [Key] | HasKey | Id, EntityId
| [Column] | HasColumnName | PropertyName
| [Required] | IsRequired | ValueType
| [MaxLength] | HasMaxLength | -
| [StringLength] | HasMaxLength | -

### Relationships
There are three types of relations between entities. The parent table is named as the `Principal` and the child table is named as the `Dependent` entity. If you don't specify the foreign key in an entity it will have been created in the database anyway. Those properties is named as `Shadow Properties`.

#### One to Many
There is an example of configuring one-to-many relationships between entities below.
```
public class Category
{
    public Category()
    {
        Products = new HashSet<Product>();
    }

    public int Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Product> Products { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; }

    public virtual Category Category { get; set; }
}
```

You can add data with one-to-many relations like below. Notice that the category object is assigned to the navigation property of the product object since the newly created category has no id.

```
var category = new Category { Name = "Books" };
var product = new Product { Name = "Moby Dick", UnitPrice = 21.99m, Category = category };

context.Products.Add(product);
context.SaveChanges();
```

Or you can add it like this.

```
var category = new Category
{
    Name = "Books",
    Products = new List<Product>
    {
        new Product { Name = "Moby Dick", UnitPrice = 21.99m }
    }
};

context.Categories.Add(category);
context.SaveChanges();
```

#### One to One
There is an example of configuring one-to-one relationships between entities.
```
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual CustomerAddress Address { get; set; }
}

public class CustomerAddress
{
    public int Id { get; set; }
    public string Address { get; set; }
    public string City { get; set; }

    public virtual Customer Customer { get; set; }
}
```

You can add data with one-to-one relations like below.

```
var customer = new Customer { Name = "Jane" };
var address = new CustomerAddress { Address = "Home", City = "Paris", Country = "France", Customer = customer };

var anotherCustomer = new Customer
{
    Name = "Sophia",
    Address = new CustomerAddress
    {
        Address = "Home",
        City = "London",
        Country = "United Kingdom"
    }
};

context.AddRange(address, anotherCustomer);
context.SaveChanges();
```

#### Many to Many
There is an example of configuring many-to-many relationships between entities.

```
public class Course
{
    public Course()
    {
        Students = new HashSet<Student>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public virtual ICollection<Student> Students { get; set; }
}

public class Student
{
    public Student()
    {
        Courses = new HashSet<Course>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public virtual ICollection<Course> Courses { get; set; }
}
```

Notice that Entity Framework Core created `CourseStudent` intermediate table in the database. You can add data with many-to-many relations like below.

```
var course = new Course
{
    Name = "Entity Framework",
    Students = new List<Student>
    {
        new Student { Name = "Emily"},
        new Student { Name = "Ava" }
    }
};

context.Add(course);
context.SaveChanges();
```

#### Delete Behaviors
It determines how to behave when you delete data on the parent table.

| Behavior | Description
| -------- | -----------
| Cascade | When the parent table row is removed referenced rows in the child table also will have been deleted. It is a default behavior.
| Restrict | It throws an exception when trying to delete data from the parent table that has a reference in the child table.
| SetNull | Child table's data will be set to `NULL` when you delete data in the parent table.
| NoAction | It has the same behavior with `Restrict`; performs deletion if there are no referenced rows.

You can specify the behavior with `OnDelete` method like below. If you want to use `SetNull` behavior, your foreign key property must be nullable. You can change the `DeleteBehavior` enum value according to your need.
```
modelBuilder.Entity<Category>().HasMany(m => m.Products).WithOne(w => w.Category).HasForeignKey(h => h.CategoryId).OnDelete(DeleteBehavior.NoAction);

modelBuilder.Entity<Customer>().HasOne(m => m.Address).WithOne(w => w.Customer).HasForeignKey<CustomerAddress>(f => f.Id).OnDelete(DeleteBehavior.Restrict);
```

#### Database Generated Attribute
Entity Framework Core generates value for a specific column in some scenarios, such as `IDENTITY` column. EF Core provides `DatabaseGenerated` data annotation attribute to configure how the value of a property will be generated.

| Value | Description
| ----- | -----------
| None | Value of the property will not be generated by the database. In this way you can provide your own values to id properties instead of generated ones.
| Identity | Value of the property will be generated by the database on the `INSERT` statement. This `Identity` property can't be updated. It can be `Identity`, `RowVersion`, `GUID`.
| Computed | Value of the property will be generated by the database on both `INSERT` and subsequent `UPDATE` statements. EF doesn't include computed columns in `INSERT` or `UPDATE` statements.

There is an example below for a `Computed` column.
```
modelBuilder.Entity<Product>().Property(p => p.UnitPriceWithTax).HasComputedColumnSql("[UnitPrice]*[TaxRate]");
```

From now on `UnitPriceWithTax` column will be calculated on the database. Also you have to decorate this property with data annotation attribute.
```
public decimal UnitPrice { get; set; }
public decimal TaxRate { get; set; }

[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
public decimal UnitPriceWithTax { get; set; }
```
Then you have a computed column with specification in your underlying database.

### Inheritance
Entity Framework Core allows us to design domain classes using inheritance.

#### Table per Hierarchy (TPH)
This approach suggests one table for the entire class inheritance hierarchy. The table includes a discriminator column which distinguishes between inheritance classes.

#### Table per Type (TPT)
This approach suggests a separate table for each domain class.

### Owned Entity Types
Owned Types allows you to group fields that you do not want to appear as a reference, in a separate type.

```
public class User
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
}

public class Student
{
    public int Id { get; set; }
    public string Class { get; set; } = default!;
    public string Number { get; set; } = default!;
    public User User { get; set; } = default!;
}

public class Teacher
{
    public int Id { get; set; }
    public string School { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public User User { get; set; } = default!;
}
```

Notice that the grouped reference doesn't have a primary key. The Model needs to be configured. If you don't want to use Fluent API you can decorate the `User` class with `[Owned]` data annotation attribute.

```
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Student>().OwnsOne(o => o.User);
    modelBuilder.Entity<Teacher>().OwnsOne(o => o.User, e =>
    {
        e.Property(p => p.FirstName).HasColumnName("FirstName");
        e.Property(p => p.LastName).HasColumnName("LastName");
    });
}
```

### Keyless Entity Types
Keyless entities are entities that do not have a primary key in themselves, so they cannot be tracked by EF Core and `Insert`, `Update`, `Delete` operations cannot be performed on them. Keyless Entity Types are used when mapping the data from Raw SQL, View, or tables that do not have Primary Key.

```
public class EmployeeCount
{
    public string Type { get; set; } = default!;
    public int Count { get; set; }
}
```

Context must be configured like below. If Fluent API is not used, entity must be decorated with `[Keyless]` data annotation attribute.

```
public class DataContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmployeeCount>().HasNoKey().ToView("EmployeeCount");
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<EmployeeCount> EmployeeCounts => Set<EmployeeCount>();
}
```

### Querying Strategies
These are the strategies that determine how to query data and its relational entities.

#### Eager Loading
Eager loading helps to load needed data at a single database call. With the help of the `Include` extension method we determine the data to load with a lambda expression. Then it provides `ThenInclude` extension method to load multiple levels of related data. These methods can be called as many as needed.

```
var category = context.Categories.Include(i => i.Products).ThenInclude(i => i.Features).First();
var product = context.Products.Include(i => i.Category).Include(i => i.Features).First();
```

In the above example's first line, it gets the first category and its related products and products' features. You can also load multiple related data by using projection, instead of `Include` or `ThenInclude` methods.

```
var products = context.Products.Where(s => s.CategoryId == 15).Select(s => new
{
    Product = s,
    Category = s.Category,
    Features = s.Features
}).FirstOrDefault();
```

#### Explicit Loading
Explicit loading helps to load needed data with an explicit call. An explicit call to load data is made with the `Load` method. To load single navigation property `Reference` method and to load collections `Collection` method is used.

```
var product = context.Products.First();

context.Entry(product).Reference(r => r.Category).Load();

context.Entry(product).Collection(c => c.Features).Load();
```

Result of the explicit loading can also be filtered. Since `Query` method returns the instance of `IQueryable`, you can filter the result with `Where` extension method.


```
context.Entry(product).Reference(r => r.Category).Query().Where(w => w.IsActive).Load();

context.Entry(product).Collection(c => c.Features).Query().Where(w => w.IsActive).Load();
```

It is useful when loading relevant data on a conditional basis.

#### Lazy Loading
Lazy loading loads the related data from the database when the navigation property is accessed. `Microsoft.EntityFrameworkCore.Proxies` NuGet package must be installed and enabled with `UseLazyLoadingProxies` method to use lazy-loading. Also your navigation properties must be `virtual`, otherwise it doesn't have to.

```
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
    optionsBuilder.UseLazyLoadingProxies().UseSqlServer(connectionString);
```

Or it can be enabled when using `AddDbContext` method.

```
.AddDbContext<BloggingContext>(
    c => c.UseLazyLoadingProxies()
          .UseSqlServer(myConnectionString));
```

You can get the related data by accessing it on navigation property.

```
var category = context.Categories.First();
var products = category.Products;
```

> Lazy-loading can cause unneeded extra database roundtrips.

### Indexes
Indexes are used to improve query performance and decrease the usage of memory. Indexes can be created for tables and views. There are six important index types `Clustered`, `Non-Clustered`, `Unique`, `Filtered`, `Hash` and `Columnstore`.

#### Clustered Indexes
Clustered index determines the physical order of data in a table; for this reason a table can have only one clustered index. It is a best practice for clustered index values to be unique.

#### Non-Clustered Indexes
Non-Clustered indexes perform sorting data logically, not physically. Since the non-clustered indexes is stored seperately from the actual data, a table can have more than one index.

#### Unique Indexes
Unique index is used to enforce uniqueness of key values in the index. Both Clustered and Non-Clustered indexes can be unique. By default `Primary Key` constraint creates unique clustered index.

You can add indexes with Fluent API like below.
```
modelBuilder.Entity<Customer>().HasIndex(i => i.Name).IsUnique(true);

modelBuilder.Entity<Category>().HasIndex(i => new { i.Name, i.Description });

modelBuilder.Entity<Product>().HasIndex(i => i.Name).IncludeProperties(i => new { i.UnitPrice, i.CategoryId });
```

The first row adds a unique non-clustered index to the Customer's Name column. The second row adds the composite index with the union of the Name and Description columns. The third row adds a non-clustered index with included columns. Also, you can add an index with a data annotation by marking the class with `[Index]` attribute.

### Query Log
asdasd

### Query Tags
asdasd

### Raw SQL Queries
EF Core provides `FromSqlRaw` and `FromSqlInterpolated` methods to execute raw SQL queries for the database and get the results as entity objects. When working with `FromSqlRaw` method care should be taken to prevent SQL Injection.

```
var customers = context.Customers.FromSqlRaw<Customer>("SELECT * FROM Customers").ToList();

var param = new SqlParameter("@id", 1);
var product = context.Products.FromSqlRaw("SELECT * FROM Products WHERE Id = @id", param).First();

var productId = 1;
var anotherProduct = context.Products.FromSqlInterpolated($"SELECT * FROM Products WHERE Id = {productId}").First();
```

#### Views
ToView / Model / DbSet
hasnokey performans dostu changetracker

#### Stored Procedures
fdgh

#### Functions
gfh

### Global Query Filters
Soft Delete (IsDeleted)
Multi Tenancy (TenantId) with (ctor or di with service)

