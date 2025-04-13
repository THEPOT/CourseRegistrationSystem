using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Gender { get; set; }

    public string? Image { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public Guid RoleId { get; set; }

    public virtual ICollection<AdministrativeStaff> AdministrativeStaffs { get; set; } = new List<AdministrativeStaff>();

    public virtual ICollection<Professor> Professors { get; set; } = new List<Professor>();

    public virtual ICollection<RegistrationPeriod> RegistrationPeriods { get; set; } = new List<RegistrationPeriod>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<TuitionPeriod> TuitionPeriods { get; set; } = new List<TuitionPeriod>();
}
