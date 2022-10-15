namespace EF.Inheritance.Entities;

public class CreditCard : BillingDetail
{
    public int CardType { get; set; }
    public string ExpiryMonth { get; set; } = default!;
    public string ExpiryYear { get; set; } = default!;

    public override string ToString() => $"{Owner} - Card - {Number} - {ExpiryMonth}/{ExpiryYear}";
}