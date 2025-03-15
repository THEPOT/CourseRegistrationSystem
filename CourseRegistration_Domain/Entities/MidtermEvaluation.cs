using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class MidtermEvaluation
{
    public Guid Id { get; set; }

    public Guid CourseRegistrationId { get; set; }

    public string Status { get; set; } = null!;

    public string? Recommendation { get; set; }

    public virtual CourseRegistration CourseRegistration { get; set; } = null!;
}
