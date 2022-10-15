namespace EF.Inheritance.Entities.OwnedEntityTypes;

public class Teacher
{
    public int Id { get; set; }
    public string School { get; set; } = default!;
    public string Branch { get; set; } = default!;
    public User User { get; set; } = default!;
}