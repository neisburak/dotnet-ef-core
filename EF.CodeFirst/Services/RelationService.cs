using EF.CodeFirst.Data;
using EF.CodeFirst.Entities;

namespace EF.CodeFirst.Services;

public class RelationService
{
    public static void AddDataWithOneToMany(DataContext context)
    {
        if (!context.Categories.Any(a => a.Name == "Books"))
        {
            var category = new Category { Name = "Books", Description = "It contains bunch of books!" };
            var product = new Product { Name = "One Hundred Years of Solitude", UnitPrice = 21.99m, Category = category };

            context.Products.Add(product); // context.Add(product) could also be used.

            category.Products.Add(new Product { Name = "Moby Dick", UnitPrice = 15.66m, Category = category });
            category.Products.Add(new Product { Name = "The Great Gatsby", UnitPrice = 15.66m, Category = category });

            context.Categories.Add(category); // context.Add(category) could also be used.

            context.SaveChanges();
        }
    }

    public static void AddDataWithOneToOne(DataContext context)
    {
        if (!context.Customers.Any(a => a.Name == "Jane"))
        {
            var customer = new Customer { Name = "Jane" };
            var address = new CustomerAddress { Address = "Home", City = "Paris", Country = "France", Customer = customer };

            var anotherCustomer = new Customer
            {
                Name = "Sophia",
                Address = new CustomerAddress { Address = "Home", City = "London", Country = "United Kingdom" }
            };

            context.AddRange(address, anotherCustomer);
            context.SaveChanges();
        }
    }

    public static void AddDataWithManyToMany(DataContext context)
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
}