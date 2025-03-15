using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Image { get; set; }

    public Guid RoleId { get; set; }

    public virtual ICollection<AdministrativeStaff> AdministrativeStaffs { get; set; } = new List<AdministrativeStaff>();

    public virtual ICollection<Professor> Professors { get; set; } = new List<Professor>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
