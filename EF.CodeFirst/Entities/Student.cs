namespace EF.CodeFirst.Entities;

public class Student
{
    public Student()
    {
        Courses = new HashSet<Course>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public virtual ICollection<Course> Courses { get; set; }
}