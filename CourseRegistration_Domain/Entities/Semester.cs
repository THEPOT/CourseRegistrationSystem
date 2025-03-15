using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class Semester
{
    public Guid Id { get; set; }

    public string SemesterName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();

    public virtual ICollection<StudentTuition> StudentTuitions { get; set; } = new List<StudentTuition>();
}
