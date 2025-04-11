using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class FinancialAid
{
    public Guid Id { get; set; }

    public string AidName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public string? EligibilityCriteria { get; set; }

    public DateOnly? ApplicationDeadline { get; set; }

    public Guid? DepartmentId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<StudentFinancialAid> StudentFinancialAids { get; set; } = new List<StudentFinancialAid>();
}
