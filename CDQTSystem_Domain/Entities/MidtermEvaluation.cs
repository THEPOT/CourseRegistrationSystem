using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class MidtermEvaluation
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Guid ProfessorId { get; set; }

    public Guid CourseId { get; set; }

    public Guid ClassSectionId { get; set; }

    public Guid SemesterId { get; set; }

    public decimal? Score { get; set; }

    public string? Comments { get; set; }

    public string? Recommendation { get; set; }

    public DateTime EvaluationDate { get; set; }

    public virtual ClassSection ClassSection { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual Professor Professor { get; set; } = null!;

    public virtual Semester Semester { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
