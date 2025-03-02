using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class CourseEvaluation
{
    public Guid Id { get; set; }

    public Guid CourseOfferingId { get; set; }

    public Guid StudentId { get; set; }

    public int Rating { get; set; }

    public string? Comments { get; set; }

    public DateTime EvaluationDate { get; set; }

    public virtual CourseOffering CourseOffering { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
