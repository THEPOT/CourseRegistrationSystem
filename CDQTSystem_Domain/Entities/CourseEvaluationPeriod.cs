using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class CourseEvaluationPeriod
{
    public Guid Id { get; set; }

    public Guid SemesterId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Semester Semester { get; set; } = null!;
}
