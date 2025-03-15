using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class Classroom
{
    public Guid Id { get; set; }

    public string RoomName { get; set; } = null!;

    public int Capacity { get; set; }

    public string? Location { get; set; }

    public string? Status { get; set; }

    public string? Equipment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();
}
