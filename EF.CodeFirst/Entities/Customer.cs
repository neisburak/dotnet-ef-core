namespace EF.CodeFirst.Entities;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public virtual CustomerAddress Address { get; set; } = default!;
}