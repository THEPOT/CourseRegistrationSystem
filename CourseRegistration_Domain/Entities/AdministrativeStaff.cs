using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class AdministrativeStaff
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid DepartmentId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
