using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class Term
{
    public Guid Id { get; set; }

    public string TermName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();

    public virtual ICollection<StudentTuition> StudentTuitions { get; set; } = new List<StudentTuition>();
}
