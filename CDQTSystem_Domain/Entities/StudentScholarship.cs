using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class StudentScholarship
{
    public Guid StudentId { get; set; }

    public Guid ScholarshipId { get; set; }

    public DateOnly AwardDate { get; set; }

    public virtual Scholarship Scholarship { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
