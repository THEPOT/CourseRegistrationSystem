using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class CourseOffering
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public Guid TermId { get; set; }

    public Guid LecturerId { get; set; }

    public string? Classroom { get; set; }

    public string? Schedule { get; set; }

    public int Capacity { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<CourseEvaluation> CourseEvaluations { get; set; } = new List<CourseEvaluation>();

    public virtual Lecturer Lecturer { get; set; } = null!;

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();

    public virtual Term Term { get; set; } = null!;
}
