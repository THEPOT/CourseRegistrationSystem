using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class Program
{
    public Guid Id { get; set; }

    public string ProgramName { get; set; } = null!;

    public int RequiredCredits { get; set; }

    public Guid FacultyId { get; set; }

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<TuitionPolicy> TuitionPolicies { get; set; } = new List<TuitionPolicy>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
