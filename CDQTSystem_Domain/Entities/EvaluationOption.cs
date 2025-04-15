using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class EvaluationOption
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public string OptionText { get; set; } = null!;

    public string? OptionTextLocalized { get; set; }

    public int OrderIndex { get; set; }

    public virtual ICollection<EvaluationAnswer> EvaluationAnswers { get; set; } = new List<EvaluationAnswer>();

    public virtual EvaluationQuestion Question { get; set; } = null!;
}
