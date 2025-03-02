using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class StudentFinancialAid
{
    public Guid StudentId { get; set; }

    public Guid FinancialAidId { get; set; }

    public DateOnly AwardDate { get; set; }

    public virtual FinancialAid FinancialAid { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
