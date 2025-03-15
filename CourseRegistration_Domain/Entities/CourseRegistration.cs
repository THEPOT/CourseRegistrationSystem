using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class CourseRegistration
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Guid ClassSectionId { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual ClassSection ClassSection { get; set; } = null!;

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<MidtermEvaluation> MidtermEvaluations { get; set; } = new List<MidtermEvaluation>();

    public virtual Student Student { get; set; } = null!;
}
