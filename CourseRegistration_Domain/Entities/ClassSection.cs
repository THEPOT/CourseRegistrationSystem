using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class ClassSection
{
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    public Guid SemesterId { get; set; }

    public Guid? ProfessorId { get; set; }

    public Guid? ClassroomId { get; set; }

    public int Capacity { get; set; }

    public bool? IsOnline { get; set; }

    public virtual ICollection<ClassSectionSchedule> ClassSectionSchedules { get; set; } = new List<ClassSectionSchedule>();

    public virtual Classroom? Classroom { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<CourseEvaluation> CourseEvaluations { get; set; } = new List<CourseEvaluation>();

    public virtual ICollection<CourseRegistration> CourseRegistrations { get; set; } = new List<CourseRegistration>();

    public virtual Professor? Professor { get; set; }

    public virtual Semester Semester { get; set; } = null!;
}
