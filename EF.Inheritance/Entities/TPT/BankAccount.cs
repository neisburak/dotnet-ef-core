namespace EF.Inheritance.Entities;

public class BankAccount : BillingDetail
{
    public string BankName { get; set; } = default!;
    public string Swift { get; set; } = default!;

    public override string ToString() => $"{Owner} - Account - {Number} - {BankName}";
}