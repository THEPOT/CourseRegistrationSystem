using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class Department
{
    public Guid Id { get; set; }

    public string DepartmentName { get; set; } = null!;

    public virtual ICollection<AdministrativeStaff> AdministrativeStaffs { get; set; } = new List<AdministrativeStaff>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<FinancialAid> FinancialAids { get; set; } = new List<FinancialAid>();

    public virtual ICollection<Major> Majors { get; set; } = new List<Major>();

    public virtual ICollection<Professor> Professors { get; set; } = new List<Professor>();

    public virtual ICollection<Scholarship> Scholarships { get; set; } = new List<Scholarship>();
}
