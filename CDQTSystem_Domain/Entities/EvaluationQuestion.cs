using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class EvaluationQuestion
{
    public Guid Id { get; set; }

    public string QuestionText { get; set; } = null!;

    public string? QuestionTextLocalized { get; set; }

    public string QuestionType { get; set; } = null!;

    public string? Category { get; set; }

    public int OrderIndex { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<EvaluationAnswer> EvaluationAnswers { get; set; } = new List<EvaluationAnswer>();

    public virtual ICollection<EvaluationOption> EvaluationOptions { get; set; } = new List<EvaluationOption>();
}
