using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class Grade
{
    public Guid Id { get; set; }

    public Guid CourseRegistrationId { get; set; }

    public string GradeValue { get; set; } = null!;

    public decimal QualityPoints { get; set; }

    public virtual CourseRegistration CourseRegistration { get; set; } = null!;
}
