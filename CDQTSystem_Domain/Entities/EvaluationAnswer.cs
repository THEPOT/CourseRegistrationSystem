using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class EvaluationAnswer
{
    public Guid Id { get; set; }

    public Guid EvaluationId { get; set; }

    public Guid QuestionId { get; set; }

    public Guid? SelectedOptionId { get; set; }

    public string? TextAnswer { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual CourseEvaluation Evaluation { get; set; } = null!;

    public virtual EvaluationQuestion Question { get; set; } = null!;

    public virtual EvaluationOption? SelectedOption { get; set; }
}
