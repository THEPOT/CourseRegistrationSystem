using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class Scholarship
{
    public Guid Id { get; set; }

    public string ScholarshipName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public string? EligibilityCriteria { get; set; }

    public DateOnly? ApplicationDeadline { get; set; }

    public Guid? DepartmentId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<StudentScholarship> StudentScholarships { get; set; } = new List<StudentScholarship>();
}
