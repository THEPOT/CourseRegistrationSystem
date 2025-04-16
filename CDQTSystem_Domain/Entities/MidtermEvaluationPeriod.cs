using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class MidtermEvaluationPeriod
{
    public Guid Id { get; set; }

    public Guid SemesterId { get; set; }

    public Guid MidtermEvaluationPeriodId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<MidtermEvaluationPeriod> InverseMidtermEvaluationPeriodNavigation { get; set; } = new List<MidtermEvaluationPeriod>();

    public virtual MidtermEvaluationPeriod MidtermEvaluationPeriodNavigation { get; set; } = null!;

    public virtual Semester Semester { get; set; } = null!;
}
