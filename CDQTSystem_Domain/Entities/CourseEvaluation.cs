using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class CourseEvaluation
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Guid ProfessorId { get; set; }

    public Guid CourseId { get; set; }

    public Guid SemesterId { get; set; }

    public DateTime EvaluationDate { get; set; }

    public string? OverallSatisfaction { get; set; }

    public string? Comments { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<EvaluationAnswer> EvaluationAnswers { get; set; } = new List<EvaluationAnswer>();

    public virtual Professor Professor { get; set; } = null!;

    public virtual Semester Semester { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
