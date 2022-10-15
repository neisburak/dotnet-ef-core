using System.ComponentModel.DataAnnotations.Schema;

namespace EF.CodeFirst.Entities;

[Table("CategoryTable", Schema = "Products")]
public class Category
{
    public Category()
    {
        Products = new HashSet<Product>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;

    public virtual ICollection<Product> Products { get; set; }

    public override string ToString() => $"{Id} - {Name} - {Description}";
}