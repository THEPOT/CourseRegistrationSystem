using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class TuitionPolicy
{
    public Guid Id { get; set; }

    public string PolicyName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public Guid? ProgramId { get; set; }

    public virtual Program? Program { get; set; }

    public virtual ICollection<StudentTuition> StudentTuitions { get; set; } = new List<StudentTuition>();
}
