using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class Professor
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid DepartmentId { get; set; }

    public virtual ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();

    public virtual Department Department { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
