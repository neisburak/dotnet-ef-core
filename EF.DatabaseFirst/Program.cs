using EF.DatabaseFirst.Data;
using Microsoft.EntityFrameworkCore;

using var context = new NorthwindDbContext();

var products = await context.Products.Take(10).ToListAsync();

products.ForEach(f => Console.WriteLine(f));