namespace EF.CodeFirst.Entities;

public class Course
{
    public Course()
    {
        Students = new HashSet<Student>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public virtual ICollection<Student> Students { get; set; }
}