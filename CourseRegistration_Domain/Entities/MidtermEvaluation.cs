using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class MidtermEvaluation
{
    public Guid Id { get; set; }

    public Guid RegistrationId { get; set; }

    public string Status { get; set; } = null!;

    public string? Recommendation { get; set; }

    public virtual Registration Registration { get; set; } = null!;
}
