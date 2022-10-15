namespace EF.CodeFirst.Entities;

public class Product
{
    public Product()
    {
        Features = new HashSet<ProductFeature>();
    }

    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }

    public virtual Category Category { get; set; } = default!;
    public virtual ICollection<ProductFeature> Features { get; set; }

    public override string ToString() => $"{Id}\t{Name}\t{UnitPrice}";
}
