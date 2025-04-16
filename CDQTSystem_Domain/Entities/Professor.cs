using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class Professor
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid DepartmentId { get; set; }

    public virtual ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();

    public virtual ICollection<CourseEvaluation> CourseEvaluations { get; set; } = new List<CourseEvaluation>();

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<MidtermEvaluation> MidtermEvaluations { get; set; } = new List<MidtermEvaluation>();

    public virtual User User { get; set; } = null!;
}
