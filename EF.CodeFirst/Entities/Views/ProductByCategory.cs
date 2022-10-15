namespace EF.CodeFirst.Entities.Views;

public class ProductByCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
    public decimal UnitPrice { get; set; }

    public override string ToString() => $"{nameof(ProductByCategory)} = {CategoryName} - {Name} - {UnitPrice}";
}