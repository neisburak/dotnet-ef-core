namespace EF.CodeFirst.Entities;

public class CustomerAddress
{
    public int Id { get; set; }
    public string Address { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Country { get; set; } = default!;

    public virtual Customer Customer { get; set; } = default!;
}