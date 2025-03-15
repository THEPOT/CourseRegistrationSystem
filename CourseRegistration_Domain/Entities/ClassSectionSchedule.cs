using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class ClassSectionSchedule
{
    public Guid Id { get; set; }

    public Guid ClassSectionId { get; set; }

    public string DayOfWeek { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual ClassSection ClassSection { get; set; } = null!;
}
