﻿using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class StudentTuition
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Guid TermId { get; set; }

    public Guid TuitionPolicyId { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal AmountPaid { get; set; }

    public decimal? DiscountAmount { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public DateOnly PaymentDueDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? Notes { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Term Term { get; set; } = null!;

    public virtual TuitionPolicy TuitionPolicy { get; set; } = null!;
}
