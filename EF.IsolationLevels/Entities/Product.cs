namespace EF.CodeFirst.Entities;

public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = default!;
    public decimal UnitPrice { get; set; }

    public virtual Category Category { get; set; } = default!;

    public override string ToString() => $"{Id}\t{Name}\t{UnitPrice}";
}
