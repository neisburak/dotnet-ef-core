namespace EF.Concurrency.Entities;

public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public byte[] RowVersion { get; set; } = default!;

    public virtual Category Category { get; set; } = default!;
}
