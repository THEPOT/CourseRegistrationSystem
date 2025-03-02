using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class Grade
{
    public Guid Id { get; set; }

    public Guid RegistrationId { get; set; }

    public string GradeValue { get; set; } = null!;

    public decimal QualityPoints { get; set; }

    public virtual Registration Registration { get; set; } = null!;
}
