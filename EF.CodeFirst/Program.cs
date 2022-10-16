using System.Data;
using EF.CodeFirst.Data;
using EF.CodeFirst.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using var context = new NorthwindDbContext();

Console.WriteLine($"ContextId: {context.ContextId}"); // cdf8f23e-27dd-4f9a-980c-520a139cb180:0

if (!context.Products.Any() && !context.Categories.Any()) await Seed(context);

await WriteProducts(context);

WriteTrackingProducts(context);

var productsWithNotTracked = await context.Products.AsNoTracking().ToListAsync(); // Data will not be tracked.

UsageOfSomeMethods(context);

// Relations
AddDataWithOneToManyRelation(context);
AddDataWithOneToOneRelation(context);
AddDataWithManyToManyRelation(context);

// Querying
QueryDataWithEagerLoading(context);
QueryDataWithExplicitLoading(context);
QueryDataWithLazyLoading(context);

// Join Operations
QueryDataWithInnerJoin(context);
QueryDataWithLeftJoin(context);
QueryDataWithOuterJoin(context);

// Raw SQL Queries
QueryDataWithSQLQueries(context);

// Paginate
PaginateCategories(context, page: 1, pageSize: 10).ForEach(f => Console.WriteLine(f));

// Query Tag
QueryDataWithTag(context);

// Global Query Filters
QueryDataWithFilters(context);

// Views
QueryDataWithViews(context);

// Stored Procedures
QueryDataWithProcedures(context);

// Functions
QueryDataWithFunctions(context);

// Transaction
TransactionExample(context);



async Task Seed(NorthwindDbContext context)
{
    var categories = new List<Category>
    {
        new() { Name = "Books" },
        new() { Name = "Computers" },
    };

    var products = new List<Product>
    {
        new() { Name = "Ulysses", UnitPrice = 15.66m, Category = categories.First(), Features = new List<ProductFeature> { new() { Key = "Paperback", Value = "228" } } },
        new() { Name = "It starts with us", UnitPrice = 25.66m, Category = categories.First(), Features = new List<ProductFeature> { new() { Key = "Paperback", Value = "315" } } },
        new() { Name = "Harry Potter", UnitPrice = 16.66m, Category = categories.First(), Features = new List<ProductFeature> { new() { Key = "Paperback", Value = "412" } } },
        new() { Name = "Atomic Habits", UnitPrice = 24.99m, Category = categories.First(), Features = new List<ProductFeature> { new() { Key = "Paperback", Value = "289" } } },
        new() { Name = "MacBook", UnitPrice = 315.66m, Category = categories.Last(), Features = new List<ProductFeature> { new() { Key = "CPU", Value = "M1" } } },
        new() { Name = "Laptop", UnitPrice = 215.66m, Category = categories.Last(), Features = new List<ProductFeature> { new() { Key = "CPU", Value = "Intel" } } },
    };

    await context.AddRangeAsync(products);

    await context.SaveChangesAsync();

    //var state = context.Entry(product).State;
    // context.Entry(product).State = EntityState.Added;
}

async Task WriteProducts(NorthwindDbContext context)
{
    var products = await context.Products.ToListAsync(); // Each data will be tracked since AsNoTracking isn't used.

    products.ForEach(f => Console.WriteLine($"{f} - State: {context.Entry(f).State}"));
}

void WriteTrackingProducts(NorthwindDbContext context)
{
    var entries = context.ChangeTracker.Entries().ToList(); // Gets tracked Entities
    entries.ToList().ForEach(f =>
    {
        if (f.Entity is Product product) Console.WriteLine(product);
    });
}

void UsageOfSomeMethods(NorthwindDbContext context)
{
    try
    {
        var firstProduct = context.Products.First(); // Throws an error if there is no data
        var firstOrDefaultProduct = context.Products.FirstOrDefault(f => f.Id == 100);
        var firstOrDefaultProductWithDefaultValue = context.Products.FirstOrDefault(f => f.Id == 100);
        var singleProduct = context.Products.Single(s => s.Id == 3);
        var filteredProducts = context.Products.Where(w => w.CategoryId == 1 && w.UnitPrice > 100).ToList();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
    }
}

void AddDataWithOneToManyRelation(NorthwindDbContext context)
{
    if (!context.Categories.Any(a => a.Name == "Books"))
    {
        var category = new Category { Name = "Books", Description = "It contains bunch of books!" };
        var product = new Product { Name = "One Hundred Years of Solitude", UnitPrice = 21.99m, Category = category };

        context.Products.Add(product); // context.Add(product);

        category.Products.Add(new Product { Name = "Moby Dick", UnitPrice = 15.66m, Category = category });
        category.Products.Add(new Product { Name = "The Great Gatsby", UnitPrice = 15.66m, Category = category });

        context.Categories.Add(category); // context.Add(category);

        context.SaveChanges();
    }
}
void AddDataWithOneToOneRelation(NorthwindDbContext context)
{
    if (!context.Customers.Any(a => a.Name == "Jane"))
    {
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
    }
}
void AddDataWithManyToManyRelation(NorthwindDbContext context)
{
    if (!context.Courses.Any(a => a.Name == "Entity Framework"))
    {
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
    }
}

void QueryDataWithEagerLoading(NorthwindDbContext context)
{
    Console.WriteLine("Eeager Loading Example");
    // It includes Products then its Features. You can use Include and ThenInclude multiple times.
    var category = context.Categories.Include(i => i.Products).ThenInclude(i => i.Features).First();
    if (category is not null)
    {
        category.Products.ToList().ForEach(product =>
        {
            Console.WriteLine($"{category.Name}\t{product.Name}\t{product.UnitPrice}");
        });
    }
}
void QueryDataWithExplicitLoading(NorthwindDbContext context)
{
    Console.WriteLine("Explicit Loading Example");
    var product = context.Products.FirstOrDefault();

    if (product is not null)
    {
        context.Entry(product).Reference(r => r.Category).Load(); // Loads single navigation property
        context.Entry(product).Reference(r => r.Category).Query().Where(w => w.Id == 1).Load(); // Usage with Query and Where methods
        context.Entry(product).Collection(c => c.Features).Load(); // Loads collection

        Console.WriteLine($"{product.Name} product in the {product.Category.Name} category and has {product.Features.Count} features.");
    }
}
void QueryDataWithLazyLoading(NorthwindDbContext context)
{
    Console.WriteLine("Lazy Loading Example");
    var category = context.Categories.First();

    category.Products.ToList().ForEach(f => Console.WriteLine(f));
}

void QueryDataWithInnerJoin(NorthwindDbContext context)
{
    var result = context.Products.Join(context.Categories, x => x.CategoryId, y => y.Id, (p, c) => new
    {
        Id = p.Id,
        CategoryName = c.Name,
        Name = p.Name,
        UnitPrice = p.UnitPrice
    });

    var resultWithMultipleJoin = result.Join(context.ProductFeatures, x => x.Id, y => y.ProductId, (p, f) => new
    {
        Id = p.Id,
        CategoryName = p.CategoryName,
        Name = p.Name,
        UnitPrice = p.UnitPrice,
        Feature = new { Key = f.Key, Value = f.Value }
    });

    var resultWithQuery = from category in context.Categories
                          join product in context.Products on category.Id equals product.CategoryId
                          select new
                          {
                              Id = product.Id,
                              CategoryName = category.Name,
                              Name = product.Name,
                              UnitPrice = product.UnitPrice
                          };

    resultWithQuery.ToList().ForEach(f => Console.WriteLine($"{f.CategoryName} - {f.Name} - {f.UnitPrice}"));
}
void QueryDataWithLeftJoin(NorthwindDbContext context)
{
    var query = from product in context.Products
                join category in context.Categories on product.CategoryId equals category.Id
                join feature in context.ProductFeatures on product.Id equals feature.ProductId into productFeatures
                from productFeature in productFeatures.DefaultIfEmpty()
                select new
                {
                    Id = product.Id,
                    CategoryName = category.Name,
                    Name = product.Name,
                    UnitPrice = product.UnitPrice,
                    Feature = productFeature != null ? new { Key = productFeature.Key, Value = productFeature.Value } : null,
                };

    query.ToList().ForEach(f => Console.WriteLine($"{f.CategoryName} - {f.Name} - {f.UnitPrice} - {(f.Feature is not null ? $"{f.Feature!.Key}: {f.Feature.Value}" : "No feature(s) added")}"));
}
void QueryDataWithOuterJoin(NorthwindDbContext context)
{
    var leftQuery = from product in context.Products
                    join feature in context.ProductFeatures on product.Id equals feature.ProductId into features
                    from feature in features.DefaultIfEmpty()
                    select new
                    {
                        Product = product,
                        Feature = feature
                    };

    var rightQuery = from feature in context.ProductFeatures
                     join product in context.Products on feature.ProductId equals product.Id into products
                     from product in products.DefaultIfEmpty()
                     select new
                     {
                         Product = product,
                         Feature = feature
                     };

    var fullOuter = leftQuery.Union(rightQuery);

    var result = fullOuter.ToList();
    Console.WriteLine($"Full outer result count: {result.Count}");
}

void QueryDataWithSQLQueries(NorthwindDbContext context)
{
    var customers = context.Customers.FromSqlRaw<Customer>("SELECT * FROM Customers").ToList();

    var param = new SqlParameter("@id", 1);
    var product = context.Products.FromSqlRaw("SELECT * FROM Products WHERE Id = @id", param).FirstOrDefault();

    var productId = 1;
    var anotherProduct = context.Products.FromSqlInterpolated($"SELECT * FROM Products WHERE Id = {productId}").FirstOrDefault();
}

List<Category> PaginateCategories(NorthwindDbContext context, int page = 0, int pageSize = 10) =>
    context.Categories.OrderBy(o => o.Name).Skip((page - 1) * pageSize).Take(pageSize).ToList();

void QueryDataWithTag(NorthwindDbContext context)
{
    var products = context.Products.TagWith("List of products").ToList();
    products.ForEach(f => Console.WriteLine(f));
}

void QueryDataWithFilters(NorthwindDbContext context)
{
    var products = context.Products.Where(w => w.UnitPrice > 20).ToList(); // Global query filters will be applied.
    var productsWithoutFilter = context.Products.IgnoreQueryFilters().Where(w => w.UnitPrice > 20).ToList(); // Global query filters will be ignored.
}

void QueryDataWithViews(NorthwindDbContext context)
{
    var productsByCategories = context.ProductsByCategories.ToList();
    productsByCategories.ForEach(f => Console.WriteLine(f));

    var productWhere = context.ProductsByCategories.Where(w => w.UnitPrice < 20).ToList();
    productWhere.ForEach(f => Console.WriteLine(f));
}

void QueryDataWithProcedures(NorthwindDbContext context)
{
    // All returned fields must match with the DbSet<Entity>

    var category = context.Categories.FirstOrDefault();
    if (category is not null)
    {
        var param = new SqlParameter("@CategoryId", category.Id);
        var productsByCategory = context.Products.FromSqlRaw("EXEC GetProductsByCategory @CategoryId", param).IgnoreQueryFilters().ToList();
    }

    if (!context.Categories.Any(a => a.Name == "Phones"))
    {
        var categoryToAdd = new Category { Name = "Phones", Description = "Lorem ipsum dolor sit amet." };
        var outParam = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
        var result = context.Database.ExecuteSqlInterpolated($"EXEC InsertCategory {categoryToAdd.Name}, {categoryToAdd.Description}, {outParam} OUT");
        Console.WriteLine($"Category added with stored procedure. Result: {result}, Id: {outParam.Value}");
    }
}

void QueryDataWithFunctions(NorthwindDbContext context)
{
    // Table Valued Functions
    var categories = context.SimpleCategories.ToList();
    categories.ForEach(f => Console.WriteLine(f));

    var featuresWithMethod = context.GetProductFeatures(4).ToList();
    Console.WriteLine($"Product has {featuresWithMethod.Count} features.");

    // Scalar Valued Functions
    var categoriesWithCount = context.Categories.Select(s => new
    {
        Id = s.Id,
        Name = s.Name,
        Count = context.GetCategoriesProductCount(s.Id)
    }).ToList();
    categoriesWithCount.ForEach(f => Console.WriteLine($"{f.Name} has {f.Count} products."));
}

void TransactionExample(NorthwindDbContext context)
{
    using var transaction = context.Database.BeginTransaction();
    try
    {
        var category = new Category { Name = "Clothes" };
        context.Categories.Add(category);
        context.SaveChanges();

        var product = new Product { Name = "Jacket", UnitPrice = 24.99m, CategoryId = int.MaxValue };
        context.Products.Add(product);
        context.SaveChanges();

        transaction.Commit();
    }
    catch (Exception ex)
    {
        transaction.Rollback();
        Console.WriteLine($"Exception: {ex.Message}");
    }
}


