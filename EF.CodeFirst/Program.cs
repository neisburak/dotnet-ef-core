using EF.CodeFirst.Data;
using EF.CodeFirst.Entities;
using EF.CodeFirst.Services;
using Microsoft.EntityFrameworkCore;

using var context = new DataContext();

// Common Features
Write($"ContextId: {context.ContextId}");  // cdf8f23e-27dd-4f9a-980c-520a139cb180:0

await UtilityService.SeedContext(context);


// ChangeTracker
var trackingProducts = ChangeTrackerService.GetProducts(context, page: 1, pageSize: 10); // Each data will be tracked since AsNoTracking isn't used.
WriteItems(trackingProducts);

var untrackedProducts = ChangeTrackerService.GetProductsAsNoTracking(context); // Data will not be tracked since AsNoTracked method used.
WriteItems(untrackedProducts);

var product = trackingProducts.First();
var state = context.Entry(product).State; // Getting entity state

var category = new Category { Name = "Videos" };
context.Entry(category).State = EntityState.Added; // Setting entity state 

var trackedEntities = ChangeTrackerService.GetTrackedEntities(context);
trackedEntities.ForEach(f =>
{
    if (f.Entity is Product product) Console.WriteLine(product); // Writing the tracked entities of Product type
});



// Relations
RelationService.AddDataWithOneToOne(context); // 1-1
RelationService.AddDataWithOneToMany(context); // 1-n
RelationService.AddDataWithManyToMany(context); // n-n



// Querying Strategies
var eagerLoadedData = QueryingService.GetDataWithEagerLoading(context);
WriteItems(eagerLoadedData.Products.ToList());

var explicitlyLoadedData = QueryingService.GetDataWithExplicitLoading(context);
Write($"{explicitlyLoadedData.Name} product in the {explicitlyLoadedData.Category.Name} category and has {explicitlyLoadedData.Features.Count} features.");

var lazyLoadedData = QueryingService.GetDataWithLazyLoading(context);
WriteItems(lazyLoadedData);



// Join Operations
var innerJoinResult = JoinService.GetDataWithInnerJoin(context);
innerJoinResult.ForEach(f => Write($"{f.CategoryName} - {f.Name} - {f.UnitPrice}"));

var leftJoinResult = JoinService.GetDataWithLeftJoin(context);
leftJoinResult.ForEach(f => Write($"{f.CategoryName} - {f.Name} - {f.UnitPrice} - {(f.Feature.HasValue ? $"{f.Feature?.Key}: {f.Feature?.Value}" : "No feature(s) added")}"));

var fullOuterJoinResult = JoinService.GetDataWithFullOuterJoin(context);
fullOuterJoinResult.ForEach(f => Write($"{f.CategoryName} - {f.ProductName}"));



// Query Tags
var products = context.Products.TagWith("List of products").ToList();
products.ForEach(f => Console.WriteLine(f));



// Global Query Filters
var productWithFilter = GlobalQueryFilterService.GetProducts(context); // Global query filters will be applied since the filter is defined in DataContext class.
var productsWithoutFilter = GlobalQueryFilterService.GetProductsByIgnoreQueryFilters(context); // Global query filters will be ignored.



// Raw SQL Queries
var customers = RawSQLService.GetCustomersFromSqlRaw(context); // All the fields returned in the sql statement must match with the entity of DbSet<Entity>
var productFromSql = RawSQLService.GetProductFromSqlRaw(context);
var categoryFromSql = RawSQLService.GetCategoryFromSqlInterpolated(context, id: 1);


// Views
var productsByCategories = context.ProductsByCategories.Where(w => w.UnitPrice < 20).ToList(); // Specified filter will be merged into the generated SQL query
productsByCategories.ForEach(f => Console.WriteLine(f));



// Stored Procedures
var productsWithProcedure = StoredProcedureService.GetProductsWithProcedure(context); // All returned fields must match with the DbSet<Entity>
var addedCategoryId = StoredProcedureService.InsertCategoryWithProcedure(context);



// Functions
// 1 - Table Valued Functions
var categories = context.SimpleCategories.ToList();
var featuresWithMethod = context.GetProductFeatures(4).ToList();

// 2 - Scalar Valued Functions
var categoriesWithCount = context.Categories.Select(s => new
{
    Id = s.Id,
    Name = s.Name,
    Count = context.GetCategoriesProductCount(s.Id)
}).ToList();



// Transactions
var result = TransactionService.PerformTransaction(context);
Console.WriteLine(result ? "succ" : "fail");



// Helper Methods
void Write(object value) => Console.WriteLine(value);
void WriteItems<T>(List<T> items) where T : new() => items.ForEach(e => Console.WriteLine(e));