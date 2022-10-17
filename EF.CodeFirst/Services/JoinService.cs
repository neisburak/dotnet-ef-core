using EF.CodeFirst.Data;
using EF.CodeFirst.Models;

namespace EF.CodeFirst.Services;

public class JoinService
{
    public static List<ProductWithPrice> GetDataWithInnerJoin(DataContext context)
    {
        // Using with extension methods
        // var result = context.Products.Join(context.Categories, x => x.CategoryId, y => y.Id, (p, c) => new
        // {
        //     Id = p.Id,
        //     CategoryName = c.Name,
        //     Name = p.Name,
        //     UnitPrice = p.UnitPrice
        // });

        // Using with multiple join
        // var resultWithMultipleJoin = result.Join(context.ProductFeatures, x => x.Id, y => y.ProductId, (p, f) => new
        // {
        //     Id = p.Id,
        //     CategoryName = p.CategoryName,
        //     Name = p.Name,
        //     UnitPrice = p.UnitPrice,
        //     Feature = new { Key = f.Key, Value = f.Value }
        // });

        var resultWithQuery = from category in context.Categories
                              join product in context.Products on category.Id equals product.CategoryId
                              select new ProductWithPrice(product.Id, category.Name, product.Name, product.UnitPrice);

        return resultWithQuery.ToList();
    }

    public static List<ProductWithFeature> GetDataWithLeftJoin(DataContext context)
    {
        var query = from product in context.Products
                    join category in context.Categories on product.CategoryId equals category.Id
                    join feature in context.ProductFeatures on product.Id equals feature.ProductId into productFeatures
                    from productFeature in productFeatures.DefaultIfEmpty()
                    select new ProductWithFeature(product.Id, category.Name, product.Name, product.UnitPrice, productFeature != null ? new KeyValuePair<string, string>(productFeature.Key, productFeature.Value) : null);

        return query.ToList();
    }

    public static List<ProductWithCategory> GetDataWithFullOuterJoin(DataContext context)
    {
        var leftQuery = from category in context.Categories
                        join product in context.Products on category.Id equals product.CategoryId into products
                        from product in products.DefaultIfEmpty()
                        select new
                        {
                            ProductName = product.Name,
                            CategoryName = category.Name
                        };

        // Suppose that a single product can exist without a category.
        var rightQuery = from product in context.Products
                         join category in context.Categories on product.CategoryId equals category.Id into categories
                         from category in categories.DefaultIfEmpty()
                         select new
                         {
                             ProductName = product.Name,
                             CategoryName = category.Name
                         };

        var fullOuter = leftQuery.Union(rightQuery);

        return fullOuter.Select(s => new ProductWithCategory(s.ProductName, s.CategoryName)).ToList();
    }
}