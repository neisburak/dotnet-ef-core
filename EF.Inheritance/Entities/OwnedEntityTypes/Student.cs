namespace EF.Inheritance.Entities.OwnedEntityTypes;

public class Student
{
    public int Id { get; set; }
    public string Class { get; set; } = default!;
    public string Number { get; set; } = default!;
    public User User { get; set; } = default!;
}