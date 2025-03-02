using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class Lecturer
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid FacultyId { get; set; }

    public virtual ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
