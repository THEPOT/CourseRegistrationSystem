using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class Semester
{
    public Guid Id { get; set; }

    public string SemesterName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string AcademicYear { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();

    public virtual ICollection<CourseEvaluationPeriod> CourseEvaluationPeriods { get; set; } = new List<CourseEvaluationPeriod>();

    public virtual ICollection<CourseEvaluation> CourseEvaluations { get; set; } = new List<CourseEvaluation>();

    public virtual ICollection<MidtermEvaluationPeriod> MidtermEvaluationPeriods { get; set; } = new List<MidtermEvaluationPeriod>();

    public virtual ICollection<MidtermEvaluation> MidtermEvaluations { get; set; } = new List<MidtermEvaluation>();

    public virtual ICollection<RegistrationPeriod> RegistrationPeriods { get; set; } = new List<RegistrationPeriod>();

    public virtual ICollection<StudentTuition> StudentTuitions { get; set; } = new List<StudentTuition>();

    public virtual ICollection<TuitionPeriod> TuitionPeriods { get; set; } = new List<TuitionPeriod>();
}
