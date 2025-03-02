using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class Department
{
    public Guid Id { get; set; }

    public string DepartmentName { get; set; } = null!;

    public virtual ICollection<FinancialAid> FinancialAids { get; set; } = new List<FinancialAid>();

    public virtual ICollection<Scholarship> Scholarships { get; set; } = new List<Scholarship>();

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
