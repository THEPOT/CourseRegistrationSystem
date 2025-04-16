﻿using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class Student
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid MajorId { get; set; }

    public DateOnly EnrollmentDate { get; set; }

    public DateOnly? AdmissionDate { get; set; }

    public string? AdmissionStatus { get; set; }

    public virtual ICollection<CourseEvaluation> CourseEvaluations { get; set; } = new List<CourseEvaluation>();

    public virtual ICollection<CourseRegistration> CourseRegistrations { get; set; } = new List<CourseRegistration>();

    public virtual Major Major { get; set; } = null!;

    public virtual ICollection<MidtermEvaluation> MidtermEvaluations { get; set; } = new List<MidtermEvaluation>();

    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

    public virtual ICollection<StudentFinancialAid> StudentFinancialAids { get; set; } = new List<StudentFinancialAid>();

    public virtual ICollection<StudentScholarship> StudentScholarships { get; set; } = new List<StudentScholarship>();

    public virtual ICollection<StudentTuition> StudentTuitions { get; set; } = new List<StudentTuition>();

    public virtual User User { get; set; } = null!;
}
