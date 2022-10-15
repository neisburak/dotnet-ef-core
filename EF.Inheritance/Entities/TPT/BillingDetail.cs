namespace EF.Inheritance.Entities;

public abstract class BillingDetail
{
    public int Id { get; set; }
    public string Number { get; set; } = default!;
    public string Owner { get; set; } = default!;
}