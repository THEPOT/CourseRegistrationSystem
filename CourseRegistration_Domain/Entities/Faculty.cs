using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class Faculty
{
    public Guid Id { get; set; }

    public string FacultyName { get; set; } = null!;

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Lecturer> Lecturers { get; set; } = new List<Lecturer>();

    public virtual ICollection<Program> Programs { get; set; } = new List<Program>();
}
