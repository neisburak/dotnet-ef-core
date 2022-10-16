namespace EF.CodeFirst.Entities.Functions;

public class SimpleCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int ProductCount { get; set; }

    public override string ToString() => $"{Name} category have {ProductCount} products.";
}