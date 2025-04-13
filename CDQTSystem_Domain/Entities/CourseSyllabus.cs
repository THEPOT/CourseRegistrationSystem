using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class CourseSyllabus
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public string? SyllabusContent { get; set; }

    public string? Version { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Course Course { get; set; } = null!;
}
