using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class Role
{
    public Guid Id { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
