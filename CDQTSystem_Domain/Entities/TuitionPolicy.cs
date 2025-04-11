using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class TuitionPolicy
{
    public Guid Id { get; set; }

    public string PolicyName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public DateOnly EffectiveDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public Guid? MajorId { get; set; }

    public virtual Major? Major { get; set; }

    public virtual ICollection<StudentTuition> StudentTuitions { get; set; } = new List<StudentTuition>();
}
