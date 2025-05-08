using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class ClassSession
{
    public Guid Id { get; set; }

    public Guid ClassSectionId { get; set; }

    public DateOnly Date { get; set; }

    public string DayOfWeek { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    public virtual ClassSection ClassSection { get; set; } = null!;
}
